(function ($) {
    function initComboBox($input) {
        const url = $input.data('url');
        const valueField = $input.data('value-field');
        const textField = $input.data('text-field');
        const displayFields = ($input.data('display-fields') || textField).split(',').map(f => f.trim());
        const selectedId = $input.data('selected-id');
        const placeholder = $input.data('placeholder') || '';
        const allowAdd = $input.data('allow-add') === true || $input.data('allow-add') === "true";

        const $wrapper = $input.closest('.combo-container');
        const $clearBtn = $wrapper.find('.combo-clear');
        const $dropdown = $wrapper.find('.combo-suggestions');

        $input.attr({
            'autocomplete': 'off',
            'placeholder': placeholder
        });

        let dataItems = [];

        function showClear(show) {
            $clearBtn.toggle(!!show && !!$input.val());
        }

        function renderItem(item) {
            const columns = displayFields.map(field =>
                `<div class="combo-col">${item[field] != null ? item[field] : ''}</div>`
            );
            return `<li data-id="${item[valueField]}" class="combo-item-row">${columns.join('')}</li>`;
        }

        function fetchAndRender(query) {
            if (!url) return;

            $.getJSON(url, { search: query }, function (data) {
                dataItems = data || [];

                const filtered = dataItems.filter(item =>
                    displayFields.some(field =>
                        (item[field] || '').toString().toLowerCase().includes(query)
                    )
                );

                let list = filtered.map(renderItem);

                if (filtered.length === 0 && allowAdd && query) {
                    list.push(
                        `<li data-id="new" class="combo-item-row"><div class="combo-col">➕ Add "${query}"</div></li>`
                    );
                }

                if (list.length > 0) {
                    $dropdown.html(list.join('')).show();
                } else {
                    $dropdown.hide();
                }
            });
        }

        $input.on('input focus', function () {
            const query = $input.val().toLowerCase().trim();
            showClear(true);
            fetchAndRender(query);
        });

        $dropdown.on('click', 'li.combo-item-row', function () {
            const id = $(this).data('id');

            if (id === 'new') {
                alert(`Trigger 'Add New' for: "${$input.val()}"`);
                $dropdown.hide();
                return;
            }

            const item = dataItems.find(x => String(x[valueField]) === String(id));

            if (item) {
                $input.val(item[textField] || '')
                    .data('selected-id', item[valueField])
                    .data('selected-item', item)
                    .trigger('change');
                showClear(true);
            }

            $dropdown.hide();
        });

        $clearBtn.on('click', function () {
            $input.val('')
                .removeAttr('data-selected-id')
                .removeData('selected-item')
                .trigger('change');
            $dropdown.hide();
            showClear(false);
        });

        $(document).on('click', function (e) {
            if (!$(e.target).closest($wrapper).length) {
                $dropdown.hide();
            }
        });

        // Preselect value
        if (selectedId) {
            $.getJSON(url, { id: selectedId }, function (data) {
                const selectedItem = Array.isArray(data)
                    ? data.find(x => String(x[valueField]) === String(selectedId))
                    : data;

                if (selectedItem) {
                    $input.val(selectedItem[textField] || '')
                        .data('selected-id', selectedItem[valueField])
                        .data('selected-item', selectedItem);
                    showClear(true);
                }
            });
        }
    }

    // Global initializer
    window.initComboBoxes = function () {
        $('.combo-input').each(function () {
            initComboBox($(this));
        });
    };
})(jQuery);
