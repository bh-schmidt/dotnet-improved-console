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
            string? name = null;
            string? proceed = null;
            string? color = null;
            IEnumerable<string>? foods = null;
            long? age = null;
            decimal? rate = null;

            string? lastName = null;

            using var mocker = new ConsoleMock();

            mocker.Setup()
                .ReadLineReturns(
                    "John",
                    "y",
                    "29",
                    "9.6",
                    "y",
                    "1",
                    "Mike",
                    "n")
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
                .OnReset((e) => name = null);

            form.Add()
                .TextOption("Do you want to proceed?", new[] { "y", "n" })
                .OnConfirm(result => proceed = result)
                .OnReset((e) => proceed = null);

            form.Add()
                .SingleSelect("Select your color", new[] { "red", "green", "blue" }, new() { Required = false })
                .OnConfirm(result => color = result?.Value)
                .OnReset((e) => color = null);

            form.Add()
                .MultiSelect("Which of these foods do you like?", new[] { "cupcake", "pizza", "fresh fries" })
                .OnConfirm(results => foods = results.Select(e => e.Value))
                .OnReset((e) => foods = null);

            form.Add()
                .LongField("How old are you?")
                .OnConfirm(value => age = value);

            form.Add()
                .DecimalField("Rate your profession")
                .OnConfirm(value => rate = value);

            form.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
    @"(1) What is your name?
 - Mike
(2) Do you want to proceed?
 - y
(3) Select your color
 - No value selected
(4) Which of these foods do you like?
 - cupcake, pizza, fresh fries
(5) How old are you?
 - 29
(6) Rate your profession
 - 9.6

Do you want to edit something? (y/n)
n
");

            lastName.Should().Be("John");
            name.Should().Be("Mike");
            proceed.Should().Be("y");
            color.Should().BeNull();
            foods.Should().BeEquivalentTo("cupcake", "pizza", "fresh fries");
            age.Should().Be(29);
            rate.Should().Be(9.6M);
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
                    "y",
                    "next.js",
                    "y",
                    "1",
                    "y",
                    "rust",
                    "n")
                .ReadKeyReturns(
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,

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
                        studyPossibilities = new[] { "y", "n" };
                        passionPossibilities = new[] { "javascript", "css", "html" };
                        return;
                    }

                    technologyPossibilities = new[] { "c#", "java", "node" };
                    studyPossibilities = new[] { "y", "n" };
                    passionPossibilities = new[] { "microservices", "events", "caching" };
                });

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .MultiSelect("Wich techlonologies do you use?", () => technologyPossibilities)
                .OnConfirm(results => technoligies = results.Select(e => e.Value))
                .OnConfirm(results => lastTechnoligies ??= results.Select(e => e.Value));

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .TextOption("Do you study other technologies?", () => studyPossibilities)
                .OnConfirm(result => study = result)
                .OnConfirm(result => lastStudy ??= result);

            form.Add(new FormItemOptions { Dependencies = [areaField] })
               .SingleSelect("Which do you like more?", () => passionPossibilities)
               .OnConfirm(result => taste = result?.Value)
               .OnConfirm(result => lastTaste ??= result?.Value);

            form.Add(new FormItemOptions { Dependencies = [areaField] })
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
 - y
(4) Which do you like more?
 - events
(5) What technology do you think is promising?
 - rust

Do you want to edit something? (y/n)
n
");

            area.Should().Be("backend dev");
            technoligies.Should().BeEquivalentTo("c#", "java");
            study.Should().Be("y");
            taste.Should().Be("events");
            promising.Should().Be("rust");

            lastArea.Should().Be("frontend dev");
            lastTechnoligies.Should().BeEquivalentTo("react");
            lastStudy.Should().Be("y");
            lastTaste.Should().Be("javascript");
            lastPromising.Should().Be("next.js");
        }
    }
}
