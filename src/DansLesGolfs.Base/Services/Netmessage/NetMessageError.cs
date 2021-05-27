using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base.Services.Netmessage
{
    #region NetmessageRoutingException
    public class NetmessageRoutingException : Exception
    {
        public NetmessageRoutingException(string errorCode) : base(errorCode)
        {

        }

        public static NetmessageRoutingException GetInstance(string errorCode)
        {
            return new NetmessageRoutingException(GetErrorMessage(errorCode));
        }

        private static string GetErrorMessage(string errorCode)
        {
            switch (errorCode)
            {
                case "0":
                    return "Despatch OK - Addresses for which uploading the email did not lead to any error messages.";
                case "200000":
                    return "Invalid Address - The address does not match any identifiable recipient.";
                case "200001":
                    return "Box not available - May mean full boxes.";
                case "200002":
                    return "Unknown Recip. - The domain is correct but everything before the @ doesn't correspond to any recipient.";
                case "200003":
                    return "Unknown domain - Everything after the @ does not correspond to an existing domain.";
                case "200004":
                    return "Domain not available - The recipient's remote server did not respond at the time we tried to contact it (you can also find addresses there for which the server blocked the message).";
                case "200005":
                    return "Protocol error - A connection error occurred but no DSN code was issued by the remote server.";
                case "200006":
                    return "MX error - Addresses for which the domain name does not follow declaration standards.";
                case "209001":
                    return "Message rejected - The message was rejected by the remote server.";
                case "209002":
                    return "Message blocked by MIB - The message was uploaded and accepted by the server but was blocked by software of the \"Mail In Black\" type requiring authentication of the sender.";
                case "200010":
                    return "Not delivered - Addresses for which we received an error message (not delivered) but for which we cannot define the exact manner.";
                case "900000":
                    return "Pending - Addresses pending dissemination or for which we are going to make a new attempt following an initial failure.";
                case "220000":
                    return "Abuse - Corresponds to the addresses of recipients who clicked on their webmail's \"this is spam\" button (webmail with which we have agreements for this information to be fed back to us), or who have complained to various anti- spam associations.";
                case "900202":
                    return "Your stop Emails - Addresses on your stop Email list (recipients who unsubscribed from your previous operations).";
                case "900100":
                case "900101":
                    return "Blacklist - Address not distributed because it is in Netmessage's general blacklist.";
                case "900203":
                    return "Spam blocked - Address not distributed because it contains a word that suggests it is a spamtrap (spam, abuse, postmaster).";
                case "900500":
                    return "Stop base - Duplicates found in the mailing list/lists";
                default: return "Unknow error.";
            }
        }
    }
    #endregion

    #region NetmessageTrackingException
    public class NetmessageTrackingException : Exception
    {
        public NetmessageTrackingException(string errorCode) : base(errorCode)
        {

        }

        public static NetmessageTrackingException GetInstance(string errorCode)
        {
            return new NetmessageTrackingException(GetErrorMessage(errorCode));
        }

        private static string GetErrorMessage(string errorCode)
        {
            switch (errorCode)
            {
                case "210900":
                    return "Despatch OK - Addresses for which uploading the email did not lead to any error messages.";
                case "200000":
                    return "Invalid Address - The address does not match any identifiable recipient.";
                case "200001":
                    return "Box not available - May mean full boxes.";
                case "200002":
                    return "Unknown Recip. - The domain is correct but everything before the @ doesn't correspond to any recipient.";
                case "200003":
                    return "Unknown domain - Everything after the @ does not correspond to an existing domain.";
                case "200004":
                    return "Domain not available - The recipient's remote server did not respond at the time we tried to contact it (you can also find addresses there for which the server blocked the message).";
                case "200005":
                    return "Protocol error - A connection error occurred but no DSN code was issued by the remote server.";
                case "200006":
                    return "MX error - Addresses for which the domain name does not follow declaration standards.";
                case "209001":
                    return "Message rejected - The message was rejected by the remote server.";
                case "209002":
                    return "Message blocked by MIB - The message was uploaded and accepted by the server but was blocked by software of the \"Mail In Black\" type requiring authentication of the sender.";
                case "200010":
                    return "Not delivered - Addresses for which we received an error message (not delivered) but for which we cannot define the exact manner.";
                case "900000":
                    return "Pending - Addresses pending dissemination or for which we are going to make a new attempt following an initial failure.";
                case "220000":
                    return "Abuse - Corresponds to the addresses of recipients who clicked on their webmail's \"this is spam\" button (webmail with which we have agreements for this information to be fed back to us), or who have complained to various anti- spam associations.";
                case "900202":
                    return "Your stop Emails - Addresses on your stop Email list (recipients who unsubscribed from your previous operations).";
                case "900100":
                case "900101":
                    return "Blacklist - Address not distributed because it is in Netmessage's general blacklist.";
                case "900203":
                    return "Spam blocked - Address not distributed because it contains a word that suggests it is a spamtrap (spam, abuse, postmaster).";
                case "900500":
                    return "Stop base - Duplicates found in the mailing list/lists";
                default: return "Unknow error.";
            }
        }
    }
    #endregion
}
