using Microsoft.Office.Interop.Excel;
using System;
using System.Messaging;
using System.Reflection;
using System.Text.Json;

namespace L9___MessageChannelsConstruction__Excel_
{
    class Router
    {
        private MessageQueue messageQueue;

        private Application oXL;
        private _Workbook oWB;
        private _Worksheet oSheet;
        private int lastRow = 1;

        public Router(MessageQueue messageQueue)
        {
            //Start Excel and get Application object.
            oXL = new Application();
            //Get a new workbook if the file does not exist
            string path = "C:\\Users\\Marti\\OneDrive - EFIF\\Dokumenter\\Book1.xlsx";
            if (System.IO.File.Exists(path))
            {
                oWB = oXL.Workbooks.Open(path);

                // last row
                lastRow = oWB.ActiveSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            }
            else
            {
                oWB = oXL.Workbooks.Add(Missing.Value);
            }
            // Get the active sheet
            oSheet = oWB.ActiveSheet;

            if (lastRow == 1)
            {
                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Airline Name";
                oSheet.Cells[1, 2] = "Flight No";
                oSheet.Cells[1, 3] = "Departure";
                oSheet.Cells[1, 4] = "Destination";
                oSheet.Cells[1, 5] = "Arrived At";

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "E1").Font.Bold = true;
                oSheet.get_Range("A1", "E1").VerticalAlignment = XlVAlign.xlVAlignCenter;
            }

            // setup the message queue
            this.messageQueue = messageQueue;
            this.messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
            this.messageQueue.BeginReceive();
        }

        void OnReceiveCompleted(object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = source as MessageQueue;
            Message m = mq.EndReceive(asyncResult.AsyncResult);

            m.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
            string message = m.Body as string;

            Console.WriteLine("Message: " + message);

            // Convert the message to object again
            var jsonDoc = JsonDocument.Parse(message);
            JsonElement root = jsonDoc.RootElement;

            string airlineName = root.GetProperty("CompanyName").GetString();
            string flightNo = root.GetProperty("FlightNo").GetString();
            string departure = root.GetProperty("Departure").GetString();
            string destination = root.GetProperty("Destination").GetString();
            DateTime arrived_at = root.GetProperty("ArrivedAt").GetDateTime();

            // add the data to excel
            AddToExcel(airlineName, flightNo, departure, destination, arrived_at);

            mq.BeginReceive();
        }

        private void AddToExcel(string airlineName, string flightNo, string departure, string destination, DateTime arrived_at)
        {
            // next row
            int nextRow = ++lastRow;

            // add the data
            oSheet.Cells[nextRow, 1] = airlineName;
            oSheet.Cells[nextRow, 2] = flightNo;
            oSheet.Cells[nextRow, 3] = departure;
            oSheet.Cells[nextRow, 4] = destination;
            oSheet.Cells[nextRow, 5] = arrived_at.ToString();


            //Make sure Excel is visible and give the user control of Microsoft Excel's lifetime.
            oXL.Visible = true;
            oXL.UserControl = true;

            // save sheets
            oWB.Save();
        }
    }
}