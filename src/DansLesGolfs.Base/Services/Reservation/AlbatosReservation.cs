using System;

namespace DansLesGolfs.Base.Revervation
{
    public class AlbatrosReservation : IReservable
    {
        private string url = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private string sessionID = string.Empty;
        private int clubNr = -1;
        private int role = -1;

        public AlbatrosReservation(string url, string username, string password)
        {
            this.url = url;
            this.username = username;
            this.password = password;
        }

        public bool Login(int? clubNumber = null)
        {
            string soapXml = string.Format("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:alb=\"http://albatros.services/\"><soapenv:Header/><soapenv:Body>"
                + @"<alb:doLogin>
                        <request>
                            <anonymous>false</anonymous>" + (clubNumber.HasValue ? "<clubNr>" + clubNumber.Value + "</clubNr>" : "") + @"
                            <login>{0}</login>
                            <password>{1}</password>
                        </request>
                    </alb:doLogin>
                </soapenv:Body>
            </soapenv:Envelope>", username, password);
            var result = SoapHelper.Execute(url, soapXml);
            if(result.code == 0 || result.code == 1)
            {
                sessionID = result.sessionInfo.sessionID;
                clubNr = (int)result.sessionInfo.clubNr;
                role = (int)result.sessionInfo.role;
                return true;
            }
            else
            {
                if (result.code == 10000)
                    throw new Exception("Access denied.");

                else if (result.code == 10004)
                    throw new Exception("Functionality is not supported in this version.");

                else if (result.code == 10101)
                    throw new Exception("Invalid password or name. No such user.");

                else if (result.code == 10102)
                    throw new Exception("Customer blocked by club.");

                else if (result.code == 10103)
                    throw new Exception("Invalid login club.");

                else if (result.code == 10104)
                    throw new Exception("Customer already have session.");

                else if (result.code == 20000)
                    throw new Exception("Invalid input data.");

                return false;
            }
        }

        public bool Logout()
        {
            string soapXml = string.Format(@"<doLogout>
                <request>
                    <sessionInfo>
                        <sessionID>{0}</sessionID>
                    </sessionInfo>
                </request>
            </doLogout>", sessionID);
            var result = SoapHelper.Execute(url, soapXml);
            return true;
        }

        public bool AddReservation(DateTime reservationDate)
        {
            return true;
        }

        public bool ConfirmReservation(int reservationId)
        {
            return true;
        }

        public bool CancelReservation(int reservationId)
        {
            return true;
        }

        public bool DeleteReservation(int reservationId)
        {
            return true;
        }
    }
}