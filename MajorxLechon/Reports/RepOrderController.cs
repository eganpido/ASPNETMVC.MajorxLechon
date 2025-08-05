using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MajorxLechon.Reports
{
    // Order - PDF
    [Authorize]
    public class RepOrderController : Controller
    {
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        public ActionResult Order(Int32 OrderId)
        {
            var currentUser = from d in db.MstUsers where d.UserId == User.Identity.GetUserId() select d;

            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document(PageSize.LETTER, 30f, 30f, 110f,140f);

            PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
            pdfWriter.PageEvent = new OrderHeaderFooter(currentUser.FirstOrDefault().Id, OrderId);

            document.Open();

            Font fontArial09 = FontFactory.GetFont("Arial", 9);
            Font fontArial09Bold = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Font fontArial09Italic = FontFactory.GetFont("Arial", 09, Font.ITALIC);
            Font fontArial10 = FontFactory.GetFont("Arial", 10);
            Font fontArial10Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);

            var order = from d in db.TrnOrders where d.Id == Convert.ToInt32(OrderId) && d.IsLocked == true select d;
            if (order.Any())
            {
                String customer = order.FirstOrDefault().CustomerName;
                String landmark = order.FirstOrDefault().Landmark;
                String lookfor = order.FirstOrDefault().LookFor;
                String address = order.FirstOrDefault().Address;
                String contactNumber = order.FirstOrDefault().ContactNumber;
                String orderNo = order.FirstOrDefault().OrderNumber;
                String orderDate = order.FirstOrDefault().SalesDate.ToShortDateString();
                String deliveryDate = order.FirstOrDefault().DeliveryDate.ToShortDateString();
                String deliveryTime = order.FirstOrDefault().DeliveryTime;

                PdfPTable tableOrder = new PdfPTable(4);
                tableOrder.SetWidths(new float[] { 65f, 150f, 70f, 80f });
                tableOrder.WidthPercentage = 100;
                tableOrder.AddCell(new PdfPCell(new Phrase("Order No. : ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase(orderNo, fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase("Del. Date :", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 2 });
                tableOrder.AddCell(new PdfPCell(new Phrase(deliveryDate, fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 0 });

                tableOrder.AddCell(new PdfPCell(new Phrase("Date:  ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase(orderDate, fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase("Del. Time : ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 2 });
                tableOrder.AddCell(new PdfPCell(new Phrase(deliveryTime, fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 0 });

                tableOrder.AddCell(new PdfPCell(new Phrase("Customer : ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase(customer, fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase("Look For : ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 2 });
                tableOrder.AddCell(new PdfPCell(new Phrase(lookfor, fontArial09)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 0 });

                tableOrder.AddCell(new PdfPCell(new Phrase("Address: ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase(address, fontArial09)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase("Landmark: ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 2 });
                tableOrder.AddCell(new PdfPCell(new Phrase(landmark, fontArial09)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 0 });

                tableOrder.AddCell(new PdfPCell(new Phrase(" ", fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase(" ", fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f });
                tableOrder.AddCell(new PdfPCell(new Phrase("Contact No.: ", fontArial10Bold)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 2 });
                tableOrder.AddCell(new PdfPCell(new Phrase(contactNumber, fontArial10)) { Border = 0, PaddingTop = 3f, PaddingLeft = 5f, PaddingRight = 5f, HorizontalAlignment = 0 });

                document.Add(tableOrder);

                PdfPTable spaceTable = new PdfPTable(1);
                spaceTable.SetWidths(new float[] { 100f });
                spaceTable.WidthPercentage = 100;
                spaceTable.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f });
                document.Add(spaceTable);

                var orderItems = from d in order.FirstOrDefault().TrnOrderItems select d;
                if (orderItems.Any())
                {
                    PdfPTable tableOrderItems = new PdfPTable(4);
                    tableOrderItems.SetWidths(new float[] { 30f, 70f, 40f, 40f });
                    tableOrderItems.WidthPercentage = 100;
                    tableOrderItems.AddCell(new PdfPCell(new Phrase("Qty.", fontArial10Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 7f });
                    tableOrderItems.AddCell(new PdfPCell(new Phrase("Description", fontArial10Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 7f });
                    tableOrderItems.AddCell(new PdfPCell(new Phrase("Price", fontArial10Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 7f });
                    tableOrderItems.AddCell(new PdfPCell(new Phrase("Amount", fontArial10Bold)) { HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 7f });

                    Decimal totalAmount = 0;

                    foreach (var orderItem in orderItems)
                    {
                        tableOrderItems.AddCell(new PdfPCell(new Phrase(orderItem.Quantity.ToString("#,##0.00"), fontArial10)) { Border = 0, HorizontalAlignment = 1, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        tableOrderItems.AddCell(new PdfPCell(new Phrase(orderItem.MstItem.ItemDescription, fontArial10)) { Border = 0, HorizontalAlignment = 0, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        tableOrderItems.AddCell(new PdfPCell(new Phrase(orderItem.Price.ToString("#,##0.00"), fontArial10)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });
                        tableOrderItems.AddCell(new PdfPCell(new Phrase(orderItem.Amount.ToString("#,##0.00"), fontArial10)) { Border = 0, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 6f, PaddingLeft = 5f, PaddingRight = 5f });

                        totalAmount += orderItem.Amount;
                    }

                    tableOrderItems.AddCell(new PdfPCell(new Phrase("Total : ", fontArial10Bold)) { Colspan = 3, HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 7f, PaddingLeft = 5f, PaddingRight = 5f });
                    tableOrderItems.AddCell(new PdfPCell(new Phrase(totalAmount.ToString("#,##0.00"), fontArial10Bold)) { HorizontalAlignment = 2, PaddingTop = 3f, PaddingBottom = 7f, PaddingLeft = 5f, PaddingRight = 5f });

                    document.Add(tableOrderItems);
                    document.Add(spaceTable);
                }
                document.Add(spaceTable);

                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0F, 100.0F, BaseColor.BLACK, Element.ALIGN_MIDDLE, 2F)));
                PdfPTable tableUsers = new PdfPTable(4);
                tableUsers.SetWidths(new float[] { 100f, 100f, 100f, 100f });
                tableUsers.WidthPercentage = 100;

                tableUsers.AddCell(new PdfPCell(new Phrase("Received by : ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                tableUsers.AddCell(new PdfPCell(new Phrase(line)) { Border = 0 });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });
                tableUsers.AddCell(new PdfPCell(new Phrase(" ", fontArial10Bold)) { Border = 0, PaddingTop = 5f, PaddingBottom = 9f, PaddingLeft = 5f, PaddingRight = 5f });

                document.Add(tableUsers);
                document.Add(spaceTable);
            }

            document.Close();

            byte[] bytesStream = memoryStream.ToArray();

            memoryStream = new MemoryStream();
            memoryStream.Write(bytesStream, 0, bytesStream.Length);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, "application/pdf");
        }
    }

    // =================
    // Header and Footer
    // =================
    class OrderHeaderFooter : PdfPageEventHelper
    {
        public Int32 orderId = 0;
        public Data.majorxlechondbDataContext db;

        public OrderHeaderFooter(Int32 currentUserId, Int32 currentOrderId)
        {
            orderId = currentOrderId;

            db = new Data.majorxlechondbDataContext();
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Font fontArial09 = FontFactory.GetFont("Arial", 09);
            Font fontArial09Italic = FontFactory.GetFont("Arial", 09, Font.ITALIC);
            Font fontArial10 = FontFactory.GetFont("Arial", 10);
            Font fontArial10Bold = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Font fontArial13Bold = FontFactory.GetFont("Arial", 13, Font.BOLD);

            Paragraph headerLine = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1F, 100.0F, BaseColor.BLACK, Element.ALIGN_MIDDLE, 5F)));
            Paragraph footerLine = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0F, 100.0F, BaseColor.BLACK, Element.ALIGN_MIDDLE, 5F)));

            var defaultOrderName = "ACKNOWLEDGEMENT RECEIPT";

            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.SetWidths(new float[] { 70f, 30f });
            tableHeader.DefaultCell.Border = 0;
            tableHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tableHeader.AddCell(new PdfPCell(new Phrase(defaultOrderName, fontArial13Bold)) { Border = 0, HorizontalAlignment = 1, Colspan = 2 });
            tableHeader.AddCell(new PdfPCell(new Phrase(headerLine)) { Border = 0, Colspan = 2 });
            tableHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin) + 50, writer.DirectContent);

            PdfPTable tableFooter = new PdfPTable(2);
            tableFooter.SetWidths(new float[] { 70f, 50f });
            tableFooter.DefaultCell.Border = 0;
            tableFooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tableFooter.AddCell(new PdfPCell(new Phrase("(Printed " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt") + ") Page " + writer.PageNumber, fontArial09)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, Colspan = 2 });
            tableFooter.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin) - 50f, writer.DirectContent);
        }
    }
}