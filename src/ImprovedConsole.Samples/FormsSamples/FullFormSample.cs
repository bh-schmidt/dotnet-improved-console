using ImprovedConsole.Forms;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class FullFormSample
    {
        class User {
            public string? Name { get; set; }
            public int? Age { get; set; }
            public IEnumerable<string>? Colors { get; set; }
            public bool AddPhone { get; set; }

            public string? Country { get; set; }
            public string? Phone { get; set; }
        }

        public static void Run()
        {
            var user = new User();

            Form form = new();
            AddBasicData(user, form);
            AddPhone(user, form);

            form.Run();
        }

        private static void AddBasicData(User user, Form form)
        {
            var section = form.AddSection()
                .Name("Basic Data");

            section.Add()
                .TextField()
                .Required(true)
                .Title("What is your name?")
                .OnConfirm(value => user.Name = value);

            section.Add()
                .TextField<int>()
                .Required(true)
                .Title("What is your age?")
                .OnConfirm(value => user.Age = value);

            string[] colors = ["red", "green", "blue"];
            section.Add()
                .MultiSelect()
                .Title("What color do you like more?")
                .Options(colors)
                .OnConfirm(values =>
                {
                    user.Colors = values;
                });

            bool[] ages = [true, false,];
            section.Add()
                .SingleSelect<bool>()
                .Title("Would you like to add a phone number?")
                .ConvertToString(e => e ? "yes" : "no")
                .Options(ages)
                .OnConfirm(value => user.AddPhone = value);
        }

        private static void AddPhone(User user, Form form)
        {
            var section = form.AddSection()
                .Condition(() => user.AddPhone)
                .Name("Phone Data");

            section.Add()
                .TextField()
                .Required(true)
                .Title("What is the country of the phone number?")
                .OnConfirm(value => user.Country = value);

            section.Add()
                .TextField()
                .Required(true)
                .Title("What is the phone number?")
                .OnConfirm(value => user.Phone = value);
        }
    }
}
