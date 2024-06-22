using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    [TestFixture]
    public class RunFixture
    {
        private static IEnumerable<object> Fields() => FieldMap.Fields();
        private static IEnumerable<object> StructFieldsWithoutValues() => FieldMap
            .Fields()
            .Where(e => e.Type.IsValueType);

        [TestCaseSource(nameof(Fields))]
        public void Should_read_the_expected_value_after_trying_to_skip_the_field<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;
            T? confirmedValue = default;

            var mocker = new ConsoleMock();
            mocker.Setup()
                .ReadLine([
                    "",
                    wrapper.First!.ToString()!
                ]);

            wrapper.Field
                .Title("Type the value")
                .TransformOnRead(e =>
                {
                    preValidation = true;
                    return e;
                })
                .Validation(e =>
                {
                    validated = true;
                    return null;
                })
                .TransformOnValidate(e =>
                {
                    postValidation = true;
                    return e;
                })
                .OnConfirm(e =>
                {
                    confirmed = true;
                    confirmedValue = e;
                })
                .ValidateField()
                .Run();

            preValidation.Should().BeTrue();
            validated.Should().BeTrue();
            postValidation.Should().BeTrue();
            confirmed.Should().BeTrue();
            confirmedValue.Should().Be(wrapper.First);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value
 * This field is required.
{wrapper.First}

"""
);
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_not_be_required_and_return_the_default_value<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;
            T? confirmedValue = default;

            var mocker = new ConsoleMock();
            mocker.Setup()
                .ReadLine("");

            wrapper.Field
                .Title("Type the value")
                .Required(false)
                .TransformOnRead(e =>
                {
                    preValidation = true;
                    return e;
                })
                .Validation(e =>
                {
                    validated = true;
                    return null;
                })
                .TransformOnValidate(e =>
                {
                    postValidation = true;
                    return e;
                })
                .OnConfirm(e =>
                {
                    confirmed = true;
                    confirmedValue = e;
                })
                .ValidateField()
                .Run();

            preValidation.Should().BeFalse();
            validated.Should().BeFalse();
            postValidation.Should().BeFalse();
            confirmed.Should().BeTrue();
            confirmedValue.Should().Be(wrapper.Default);

            mocker.GetOutput()
                .Should()
                .Be(
"""
Type the value


""");
        }

        [TestCaseSource(nameof(StructFieldsWithoutValues))]
        public void Should_skip_the_field_after_having_a_conversion_error<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;
            T? confirmedValue = default;

            var mocker = new ConsoleMock();
            mocker.Setup()
                .ReadLine([
                    "Invalid conversion",
                    "",
                ]);

            wrapper.Field
                .Title("Type the value")
                .Required(false)
                .TransformOnRead(e =>
                {
                    preValidation = true;
                    return e;
                })
                .Validation(e =>
                {
                    validated = true;
                    return null;
                })
                .TransformOnValidate(e =>
                {
                    postValidation = true;
                    return e;
                })
                .OnConfirm(e =>
                {
                    confirmed = true;
                    confirmedValue = e;
                })
                .ValidateField()
                .Run();

            preValidation.Should().BeTrue();
            validated.Should().BeTrue();
            postValidation.Should().BeTrue();
            confirmed.Should().BeTrue();
            confirmedValue.Should().Be(wrapper.Default);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value
 * Could not convert the value.


"""
);
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_return_the_informed_default_because_there_was_no_selection<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;
            T? confirmedValue = default;

            var mocker = new ConsoleMock();
            mocker.Setup()
                .ReadLine([
                    string.Empty,
                ]);

            wrapper.Field
                .Title("Type the value")
                .Required(false)
                .Default(wrapper.First)
                .TransformOnRead(e =>
                {
                    preValidation = true;
                    return e;
                })
                .Validation(e =>
                {
                    validated = true;
                    return null;
                })
                .TransformOnValidate(e =>
                {
                    postValidation = true;
                    return e;
                })
                .OnConfirm(e =>
                {
                    confirmed = true;
                    confirmedValue = e;
                })
                .ValidateField()
                .Run();

            preValidation.Should().BeFalse();
            validated.Should().BeFalse();
            postValidation.Should().BeFalse();
            confirmed.Should().BeTrue();
            confirmedValue.Should().Be(wrapper.First);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value


"""
);
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_external_value_and_edit_the_value<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;
            T? confirmedValue = default;

            var mocker = new ConsoleMock();
            mocker.Setup()
                .ReadLine([
                    wrapper.First!.ToString()!
                ]);

            wrapper.Field
                .Title("Type the value")
                .Set(wrapper.Last)
                .TransformOnRead(e =>
                {
                    preValidation = true;
                    return e;
                })
                .Validation(e =>
                {
                    validated = true;
                    return null;
                })
                .TransformOnValidate(e =>
                {
                    postValidation = true;
                    return e;
                })
                .OnConfirm(e =>
                {
                    confirmed = true;
                    confirmedValue = e;
                })
                .ValidateField();

            var answer = wrapper.Field.Run().As<TextFieldAnswer<T>>();
            answer.Answer.Should().Be(wrapper.Last);

            preValidation.Should().BeFalse();
            validated.Should().BeFalse();
            postValidation.Should().BeFalse();
            confirmed.Should().BeTrue();
            mocker.GetOutput().Should().BeEmpty();
            confirmed = false;

            wrapper.Field.SetEdition();
            answer = wrapper.Field.Run().As<TextFieldAnswer<T>>();
            answer.Answer.Should().Be(wrapper.First);

            preValidation.Should().BeTrue();
            validated.Should().BeTrue();
            postValidation.Should().BeTrue();
            confirmed.Should().BeTrue();
            confirmedValue.Should().Be(wrapper.First);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value
{wrapper.First}

"""
);
        }
    }
}
