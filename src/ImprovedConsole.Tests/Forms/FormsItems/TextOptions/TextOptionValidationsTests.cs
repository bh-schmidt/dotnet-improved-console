using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Tests.Forms.FormsItems.OptionSelectors
{
    public class TextOptionValidationsTests
    {
        [Test]
        public void Should_validate_title()
        {
            Assert.Catch<TitleNotSetException>(() =>
            {
                new TextOption<string>(new FormEvents())
                    .ValidateField();
            });
        }

        [Test]
        public void Should_validate_available_options()
        {
            Assert.Catch<OptionsNotSetException>(() =>
            {
                new TextOption<string>(new FormEvents())
                    .Title("Witch color do you want?")
                    .ValidateField();
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                string[] colors = ["blue", "red"];
                new TextOption<string>(new FormEvents())
                    .Title("Witch color do you want?")
                    .Options(colors)
                    .OnConfirm(null!);
            });
        }
    }
}
