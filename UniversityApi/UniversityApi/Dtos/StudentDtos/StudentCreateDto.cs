using FluentValidation;

namespace UniversityApi.Dtos.StudentDtos
{
    public class StudentCreateDto
    {
        public int GroupId { get; set; }    
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class StudentCreateDtoValidator : AbstractValidator<StudentCreateDto>
    {
        public StudentCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().MaximumLength(35).MinimumLength(5);

            RuleFor(x => x.Email).EmailAddress().NotEmpty().MaximumLength(100).MinimumLength(5);

            RuleFor(x => x.BirthDate.Date)
                .LessThanOrEqualTo(DateTime.Now.AddYears(-15).Date)
                    .WithMessage("Student's age must be greator or equal than 15");

            RuleFor(x => x).Custom((x, c) =>
            {
            if (x.FullName!=null && !char.IsUpper(x.FullName[0]))
                {
                    c.AddFailure(nameof(x.FullName), "FullName must start with uppercase!");
                }
            });
        }
    }
}
