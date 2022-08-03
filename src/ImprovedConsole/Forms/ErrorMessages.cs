using ImprovedConsole.Forms.Fields.MultiSelects;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Forms
{
    public static class ErrorMessages
    {
        static ErrorMessages()
        {
            SingleSelect = e =>
            {
                return e switch
                {
                    SingleSelectErrorEnum.SelectionIsRequired => "{color:red}Select one option{color:red}",
                    _ => "",
                };
            };

            MultiSelect = e =>
            {
                return e switch
                {
                    MultiSelectErrorEnum.SelectionIsRequired => "{color:red}Select at least one option{color:red}",
                    _ => "",
                };
            };
        }

        public static Func<SingleSelectErrorEnum, string> SingleSelect;
        public static Func<MultiSelectErrorEnum, string> MultiSelect;
    }
}
