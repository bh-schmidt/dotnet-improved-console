using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Tests.Forms.FormsItems.SingleSelects
{
    public class SingleSelectValidationsTests
    {
        [Test]
        public void Should_throw_title_is_null_or_empty()
        {
            Assert.Catch<TitleNotSetException>(() =>
            {
                string[] colors = ["red", "green", "blue"];
                new SingleSelect<string>(new FormEvents())
                    .Options(colors)
                    .ValidateField();
            });
        }

        [Test]
        public void Should_throw_possibilities_is_null_or_empty()
        {
            Assert.Catch<OptionsNotSetException>(() =>
            {
                new SingleSelect<string>(new FormEvents())
                    .Title("What color do you pick?")
                    .ValidateField();
            });
        }
    }
}
