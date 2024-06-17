using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    public class TextFieldValidationsTests
    {
        [Test]
        public void Should_validate_the_title()
        {
            Assert.Catch<TitleNotSetException>(() =>
            {
                new TextField<string>(new FormEvents())
                    .ValidateField();
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                new TextField<string>(new FormEvents())
                    .OnConfirm(null!);
            });
        }
    }
}
