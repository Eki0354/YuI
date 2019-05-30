using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledros.Json
{
    public class JsonOutlookEmailCollection
    {

        public class Rootobject
        {
            public Header Header { get; set; }
            public Body Body { get; set; }
        }

        public class Header
        {
            public Serverversioninfo ServerVersionInfo { get; set; }
        }

        public class Serverversioninfo
        {
            public int MajorVersion { get; set; }
            public int MinorVersion { get; set; }
            public int MajorBuildNumber { get; set; }
            public int MinorBuildNumber { get; set; }
            public string Version { get; set; }
        }

        public class Body
        {
            public string __type { get; set; }
            public Responsemessages ResponseMessages { get; set; }
        }

        public class Responsemessages
        {
            public Item[] Items { get; set; }
        }

        public class Item
        {
            public string __type { get; set; }
            public Rootfolder RootFolder { get; set; }
            public object HighlightTerms { get; set; }
            public string ResponseCode { get; set; }
            public string ResponseClass { get; set; }
        }

        public class Rootfolder
        {
            public string __type { get; set; }
            public Item1[] Items { get; set; }
            public object Groups { get; set; }
            public int IndexedPagingOffset { get; set; }
            public int TotalItemsInView { get; set; }
            public bool IncludesLastItemInRange { get; set; }
        }

        public class Item1
        {
            public string __type { get; set; }
            public Sender Sender { get; set; }
            public Itemid ItemId { get; set; }
            public Parentfolderid ParentFolderId { get; set; }
            public string ItemClass { get; set; }
            public bool IsReadReceiptRequested { get; set; }
            public string Subject { get; set; }
            public string Sensitivity { get; set; }
            public string Importance { get; set; }
            public From From { get; set; }
            public DateTime DateTimeReceived { get; set; }
            public int Size { get; set; }
            public bool IsRead { get; set; }
            public bool IsSubmitted { get; set; }
            public bool IsDraft { get; set; }
            public DateTime DateTimeSent { get; set; }
            public DateTime DateTimeCreated { get; set; }
            public string DisplayTo { get; set; }
            public bool HasAttachments { get; set; }
            public Extendedproperty[] ExtendedProperty { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public Conversationid ConversationId { get; set; }
            public Flag Flag { get; set; }
            public string InstanceKey { get; set; }
            public string Preview { get; set; }
            public DateTime ReceivedOrRenewTime { get; set; }
            public int[] SystemCategories { get; set; }
            public string InferenceClassification { get; set; }
        }

        public class Sender
        {
            public Mailbox Mailbox { get; set; }
        }

        public class Mailbox
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string RoutingType { get; set; }
            public string MailboxType { get; set; }
        }

        public class Itemid
        {
            public string __type { get; set; }
            public string ChangeKey { get; set; }
            public string Id { get; set; }
        }

        public class Parentfolderid
        {
            public string __type { get; set; }
            public string Id { get; set; }
            public string ChangeKey { get; set; }
        }

        public class From
        {
            public Mailbox1 Mailbox { get; set; }
        }

        public class Mailbox1
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string RoutingType { get; set; }
            public string MailboxType { get; set; }
        }

        public class Conversationid
        {
            public string __type { get; set; }
            public string Id { get; set; }
        }

        public class Flag
        {
            public string FlagStatus { get; set; }
        }

        public class Extendedproperty
        {
            public Extendedfielduri ExtendedFieldURI { get; set; }
            public string Value { get; set; }
        }

        public class Extendedfielduri
        {
            public string __type { get; set; }
            public string PropertyTag { get; set; }
            public string PropertyType { get; set; }
        }
    }
}
