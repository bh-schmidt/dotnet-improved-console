using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.SingleSelects;
using System.Text;

namespace ImprovedConsole.Tests.Forms
{
    public class FormTests
    {
        [Test]
        public void Should_fill_the_form_and_then_fix_the_name()
        {
            Form form = new();
            string? name = null;
            string? proceed = null;
            string? color = null;
            IEnumerable<string>? foods = null;
            long? age = null;
            decimal? rate = null;

            string? lastName = null;

            using ConsoleMock mocker = new();

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
                .TextField()
                .Title("What is your name?")
                .OnConfirm(result => lastName ??= result)
                .OnConfirm(result => name = result)
                .OnReset((e) => name = null)
                .ValidateField();

            string[] confirmations = ["y", "n"];
            form.Add()
                .TextOption()
                .Title("Do you want to proceed?")
                .Options(confirmations)
                .OnConfirm(result => proceed = result)
                .OnReset((e) => proceed = null);

            string[] colors = ["red", "green", "blue"];
            form.Add()
                .SingleSelect()
                .Title("Select your color")
                .Required(false)
                .Options(colors)
                .OnConfirm(result => color = result)
                .OnReset((e) => color = null);

            string[] foodOptions = ["cupcake", "pizza", "fresh fries"];
            form.Add()
                .MultiSelect()
                .Title("Which of these foods do you like?")
                .Options(foodOptions)
                .OnConfirm(results => foods = results)
                .OnReset((e) => foods = null);

            form.Add()
                .TextField<int>()
                .Title("How old are you?")
                .OnConfirm(value => age = value);

            form.Add()
                .TextField<decimal>()
                .Title("Rate your profession")
                .OnConfirm(value => rate = value);

            form.Run();

            string output = mocker.GetOutput();

            output.Should().Be(
"""
1- What is your name?
   Mike
2- Do you want to proceed?
   y
3- Select your color
   N/A
4- Which of these foods do you like?
   cupcake, pizza, fresh fries
5- How old are you?
   29
6- Rate your profession
   9.6

Do you want to edit something? (y/n)
n

""");

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
            Form form = new();

            string? area = null;
            IEnumerable<string>? technologies = null;
            string? study = null;
            string? taste = null;
            string? promising = null;

            string? lastArea = null;
            IEnumerable<string>? lastTechnologies = null;
            string? lastStudy = null;
            string? lastTaste = null;
            string? lastPromising = null;

            using ConsoleMock mocker = new();

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

            string[] areas = ["frontend dev", "backend dev"];
            SingleSelect<string> areaField = form.Add()
                .SingleSelect()
                .Title("Which area are you in?")
                .Options(areas)
                .OnConfirm(result =>
                {
                    area = result;
                    lastArea ??= result;

                    if (area == "frontend dev")
                    {
                        technologyPossibilities = ["react", "angular", "vue"];
                        studyPossibilities = ["y", "n"];
                        passionPossibilities = ["JavaScript", "css", "html"];
                        return;
                    }

                    technologyPossibilities = ["c#", "java", "node"];
                    studyPossibilities = ["y", "n"];
                    passionPossibilities = ["microservices", "events", "caching"];
                });

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .MultiSelect()
                .Title("Which technologies do you use?")
                .Options(() => technologyPossibilities)
                .OnConfirm(results => technologies = results)
                .OnConfirm(results => lastTechnologies ??= results);

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .TextOption()
                .Title("Do you study other technologies?")
                .Options(() => studyPossibilities)
                .OnConfirm(result => study = result)
                .OnConfirm(result => lastStudy ??= result)
                .ValidateField();

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .SingleSelect()
                .Title("Which do you like more?")
                .Options(() => passionPossibilities)
                .OnConfirm(result => taste = result)
                .OnConfirm(result => lastTaste ??= result);

            form.Add(new FormItemOptions { Dependencies = [areaField] })
                .TextField()
                .Title("What technology do you think is promising?")
                .OnConfirm(result => promising = result)
                .OnConfirm(result => lastPromising ??= result)
                .ValidateField();

            form.Run();

            string output = mocker.GetOutput();

            output.Should().Be(
"""
1- Which area are you in?
   backend dev
2- Which technologies do you use?
   c#, java
3- Do you study other technologies?
   y
4- Which do you like more?
   events
5- What technology do you think is promising?
   rust

Do you want to edit something? (y/n)
n

""");

            area.Should().Be("backend dev");
            technologies.Should().BeEquivalentTo("c#", "java");
            study.Should().Be("y");
            taste.Should().Be("events");
            promising.Should().Be("rust");

            lastArea.Should().Be("frontend dev");
            lastTechnologies.Should().BeEquivalentTo("react");
            lastStudy.Should().Be("y");
            lastTaste.Should().Be("JavaScript");
            lastPromising.Should().Be("next.js");
        }

        [Test]
        public void Should_only_confirm_values_because_there_is_the_initial_value()
        {
            Form form = new();
            string? name = null;
            string? proceed = null;
            string? color = null;
            IEnumerable<string>? foods = null;
            long? age = null;
            decimal? rate = null;

            string? lastName = null;

            using ConsoleMock mocker = new();

            mocker.Setup()
                .ReadLineReturns("n");

            form.Add()
                .TextField()
                .Title("What is your name?")
                .Set("John")
                .OnConfirm(result => lastName ??= result)
                .OnConfirm(result => name = result)
                .OnReset((e) => name = null)
                .ValidateField();

            string[] confirmations = ["y", "n"];
            form.Add()
                .TextOption()
                .Title("Do you want to proceed?")
                .Options(confirmations)
                .Set("y")
                .OnConfirm(result => proceed = result)
                .OnReset((e) => proceed = null);

            string[] colors = ["red", "green", "blue"];
            form.Add()
                .SingleSelect()
                .Title("Select your color")
                .Options(colors)
                .Required(false)
                .Set("green")
                .OnConfirm(result => color = result)
                .OnReset((e) => color = null);

            string[] foodOptions = ["cupcake", "pizza", "fresh fries"];
            form.Add()
                .MultiSelect()
                .Title("Which of these foods do you like?")
                .Options(foodOptions)
                .Set(["pizza", "fresh fries"])
                .OnConfirm(results => foods = results)
                .OnReset((e) => foods = null);

            form.Add()
                .TextField<int>()
                .Title("How old are you?")
                .Set(29)
                .OnConfirm(value => age = value);

            form.Add()
                .TextField<decimal>()
                .Title("Rate your profession")
                .Set(9.6M)
                .OnConfirm(value => rate = value);

            form.Run();

            string output = mocker.GetOutput();

            output.Should().Be(
"""
1- What is your name?
   John
2- Do you want to proceed?
   y
3- Select your color
   green
4- Which of these foods do you like?
   pizza, fresh fries
5- How old are you?
   29
6- Rate your profession
   9.6

Do you want to edit something? (y/n)
n

""");

            lastName.Should().BeNull();
            name.Should().BeNull();
            proceed.Should().BeNull();
            color.Should().BeNull();
            foods.Should().BeNull();
            age.Should().BeNull();
            rate.Should().BeNull();
        }
    }
}
