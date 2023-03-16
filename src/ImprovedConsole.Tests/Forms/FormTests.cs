using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;

namespace ImprovedConsole.Tests.Forms
{
    public class FormTests
    {
        [Test]
        public void Should_fill_the_form_and_then_fix_the_name()
        {
            var form = new Form();
            string? lastName = null;
            string? name = null;
            string? proceed = null;
            string? color = null;
            IEnumerable<string>? foods = null;

            using var mocker = new ConsoleMock();

            mocker.Setup()
                .ReadLineReturns(
                    "John",
                    "yes",
                    "yes",
                    "1",
                    "Mike",
                    "no")
                .ReadKeyReturns(
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter);

            form.Add()
                .TextField("What is your name?")
                .OnConfirm(result => lastName ??= result)
                .OnConfirm(result => name = result)
                .OnReset(() => name = null);

            form.Add()
                .OptionSelector("Do you want to proceed?", new[] { "yes", "no" })
                .OnConfirm(result => proceed = result)
                .OnReset(() => proceed = null);

            form.Add()
                .SingleSelect("Select your color", new[] { "red", "green", "blue" })
                .OnConfirm(result => color = result?.Value)
                .OnReset(() => color = null);

            form.Add()
                .MultiSelect("Which of these foods do you like?", new[] { "cupcake", "pizza", "fresh fries" })
                .OnConfirm(results => foods = results.Select(e => e.Value))
                .OnReset(() => foods = null);

            form.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
    @"(1) What is your name?
 - Mike
(2) Do you want to proceed?
 - yes
(3) Select your color
 - No value selected
(4) Which of these foods do you like?
 - cupcake, pizza, fresh fries

Do you want to edit something? (yes/no)
no
");

            lastName.Should().Be("John");
            name.Should().Be("Mike");
            proceed.Should().Be("yes");
            color.Should().BeNull();
            foods.Should().BeEquivalentTo("cupcake", "pizza", "fresh fries");
        }

        [Test]
        public void Should_fill_the_form_and_then_reset_the_form()
        {
            var form = new Form();

            string? area = null;
            IEnumerable<string>? technoligies = null;
            string? study = null;
            string? taste = null;
            string? promising = null;

            string? lastArea = null;
            IEnumerable<string>? lastTechnoligies = null;
            string? lastStudy = null;
            string? lastTaste = null;
            string? lastPromising = null;

            using var mocker = new ConsoleMock();

            mocker.Setup()
                .ReadLineReturns(
                    "yes",
                    "next.js",
                    "yes",
                    "1",
                    "yes",
                    "rust",
                    "no")
                .ReadKeyReturns(
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    // edit
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter);

            string[] technologyPossibilities = null!;
            string[] studyPossibilities = null!;
            string[] passionPossibilities = null!;

            var areaField = form.Add()
                .SingleSelect("Which area are you in?", new[] { "frontend dev", "backend dev" })
                .OnConfirm(result =>
                {
                    area = result?.Value;
                    lastArea ??= area;

                    if (area == "frontend dev")
                    {
                        technologyPossibilities = new[] { "react", "angular", "vue" };
                        studyPossibilities = new[] { "yes", "no" };
                        passionPossibilities = new[] { "javascript", "css", "html" };
                        return;
                    }

                    technologyPossibilities = new[] { "c#", "java", "node" };
                    studyPossibilities = new[] { "yes", "no" };
                    passionPossibilities = new[] { "microservices", "events", "caching" };
                });

            form.Add(new FormItemOptions { Dependencies = new FormItemDependencies(areaField) })
                .MultiSelect("Wich techlonologies do you use?", () => technologyPossibilities)
                .OnConfirm(results => technoligies = results.Select(e => e.Value))
                .OnConfirm(results => lastTechnoligies ??= results.Select(e => e.Value));

            form.Add(new FormItemOptions { Dependencies = new FormItemDependencies(areaField) })
                .OptionSelector("Do you study other technologies?", () => studyPossibilities)
                .OnConfirm(result => study = result)
                .OnConfirm(result => lastStudy ??= result);

            form.Add(new FormItemOptions { Dependencies = new FormItemDependencies(areaField) })
               .SingleSelect("Which do you like more?", () => passionPossibilities)
               .OnConfirm(result => taste = result?.Value)
               .OnConfirm(result => lastTaste ??= result?.Value);

            form.Add(new FormItemOptions { Dependencies = new FormItemDependencies(areaField) })
                .TextField("What technology do you think is promising?")
                .OnConfirm(result => promising = result)
                .OnConfirm(result => lastPromising ??= result);

            form.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
            @"(1) Which area are you in?
 - backend dev
(2) Wich techlonologies do you use?
 - c#, java
(3) Do you study other technologies?
 - yes
(4) Which do you like more?
 - events
(5) What technology do you think is promising?
 - rust

Do you want to edit something? (yes/no)
no
");

            area.Should().Be("backend dev");
            technoligies.Should().BeEquivalentTo("c#", "java");
            study.Should().Be("yes");
            taste.Should().Be("events");
            promising.Should().Be("rust");

            lastArea.Should().Be("frontend dev");
            lastTechnoligies.Should().BeEquivalentTo("react");
            lastStudy.Should().Be("yes");
            lastTaste.Should().Be("javascript");
            lastPromising.Should().Be("next.js");
        }
    }
}
