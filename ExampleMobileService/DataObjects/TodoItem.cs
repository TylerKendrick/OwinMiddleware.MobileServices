using Microsoft.WindowsAzure.Mobile.Service;

namespace ExampleMobileService.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}