(function ($) {
    function initComboBox($input) {
        const url = $input.data('url');
        const valueField = $input.data('value-field');
        const textField = $input.data('text-field');
        const selectedId = $input.data('selected-id');
        const placeholder = $input.data('placeholder') || '';
        const allowAdd = $input.data('allow-add') === true || $input.data('allow-add') === "true";

        // Wrap input with .combo-wrapper and add clear button
        $input.wrap('<div class="combo-wrapper" style="position:relative;"></div>');
        const $wrapper = $input.parent();
        const $clearBtn = $('<span class="combo-clear">&times;</span>').appendTo($wrapper);
        const $dropdown = $('<ul class="dropdown-menu combo-suggestions"></ul>').appendTo($wrapper).hide();

        $input.attr({
            'autocomplete': 'off',
            'placeholder': placeholder
        });

        let dataItems = [];

        function showClear(show) {
            $clearBtn.toggle(show && !!$input.val());
        }

        function positionDropdown() {
            $dropdown.css({
                top: '100%',
                left: 0,
                width: '100%'
            });
        }

        $input.on('input focus', function () {
            const query = $input.val().toLowerCase();
            showClear(true);

            if (!url) return;

            $.getJSON(url, { search: query }, function (data) {
                dataItems = data;

                const filtered = data.filter(item =>
                    item[textField].toLowerCase().includes(query)
                );

                let list = filtered.map(item =>
                    `<li data-id="${item[valueField]}">${item[textField]}</li>`
                );

                if (filtered.length === 0 && allowAdd && query) {
                    list.push(`<li data-id="new">➕ Add "${query}"</li>`);
                }

                if (list.length) {
                    $dropdown.html(list.join(''));
                    positionDropdown();
                    $dropdown.show();
                } else {
                    $dropdown.hide();
                }
            });
        });

        $dropdown.on('click', 'li', function () {
            const text = $(this).text();
            const id = $(this).data('id');
            $input.val(text).data('selected-id', id);
            showClear(true);
            $dropdown.hide();

            if (id === 'new') {
                alert(`Trigger 'Add New' for: "${text}"`);
            }
        });

        $clearBtn.on('click', function () {
            $input.val('').data('selected-id', '');
            $dropdown.hide();
            showClear(false);
        });

        $(document).on('click', function (e) {
            if (!$(e.target).closest($wrapper).length) {
                $dropdown.hide();
            }
        });

        // Preload selected value
        if (selectedId) {
            $.getJSON(url, { id: selectedId }, function (data) {
                const selectedItem = Array.isArray(data)
                    ? data.find(x => x[valueField] == selectedId)
                    : data;

                if (selectedItem) {
                    $input.val(selectedItem[textField]).data('selected-id', selectedItem[valueField]);
                    showClear(true);
                }
            });
        }
    }

    window.initComboBoxes = function () {
        $('.combo-input').each(function () {
            initComboBox($(this));
        });
    };
})(jQuery);
