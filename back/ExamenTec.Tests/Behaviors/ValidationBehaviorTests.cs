using ExamenTec.Application.Common.Behaviors;
using FluentAssertions;
using FluentValidation;
using MediatR;

namespace ExamenTec.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_DeberiaContinuar_CuandoNoExistenValidadores()
    {
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Name = "Test" };
        var expectedResponse = new TestResponse { Result = "Success" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(expectedResponse);

        var result = await behavior.Handle(request, next, CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_DeberiaContinuar_CuandoValidacionPasa()
    {
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Name = "ValidName" };
        var expectedResponse = new TestResponse { Result = "Success" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(expectedResponse);

        var result = await behavior.Handle(request, next, CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarValidationException_CuandoValidacionFalla()
    {
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Name = "" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse());

        await Assert.ThrowsAsync<ExamenTec.Application.Common.Exceptions.ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
    }

    public class TestRequest : IRequest<TestResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio");
        }
    }
}

