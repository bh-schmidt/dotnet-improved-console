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
                .ReadLineReturns([
                    "",
                    wrapper.ValidValue!.ToString()!
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
            confirmedValue.Should().Be(wrapper.ValidValue);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value
 * This field is required.
{wrapper.ValidValue}

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
                .ReadLineReturns("");

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
                .ReadLineReturns([
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
        public void Should_only_return_the_answer_because_there_is_the_initial_value<T>(FieldWrapper<T> wrapper)
        {
            var preValidation = false;
            var validated = false;
            var postValidation = false;
            var confirmed = false;

            var mocker = new ConsoleMock();
            var answer = wrapper.Field
                .Title("Type the value")
                .Set(wrapper.ValidValue)
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
                })
                .ValidateField()
                .Run();

            var answerField = (TextFieldAnswer<T>)answer;
            answerField.Answer.Should().Be(wrapper.ValidValue);

            preValidation.Should().BeFalse();
            validated.Should().BeFalse();
            postValidation.Should().BeFalse();
            confirmed.Should().BeFalse();

            mocker.GetOutput()
                .Should()
                .BeEmpty();
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
                .ReadLineReturns([
                    string.Empty,
                ]);

            wrapper.Field
                .Title("Type the value")
                .Default(wrapper.ValidValue)
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
            confirmedValue.Should().Be(wrapper.ValidValue);

            mocker.GetOutput()
                .Should()
                .Be(
$"""
Type the value


"""
);
        }
    }
}
