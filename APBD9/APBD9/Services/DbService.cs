using APBD9.DTOs;

namespace APBD9.Services;

public interface IDbService
{
    Task<PrescriptionGetDto> CreatePrescriptionAsync(PrescriptionPostDto prescriptionData);
}

public class DbService : IDbService
{
    public async Task<PrescriptionGetDto> CreatePrescriptionAsync(PrescriptionPostDto prescriptionData)
    {
        List<Group> groups = [];

        // At first, we have to check if the given groups exist in the DB
        if (studentData.Groups is not null && studentData.Groups.Count != 0)
        {
            foreach (var groupId in studentData.Groups)
            {
                var group = await data.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group is null)
                {
                    throw new NotFoundException($"Group with id: {groupId} not found");
                }

                groups.Add(group);
            }
        }

        // *Approach 1* - add data to each table manually using multiple requests (transaction required)
        // var transaction = await data.Database.BeginTransactionAsync(); // Begin transaction
        // try
        // {
        //     var student = new Student
        //     {
        //         FirstName = studentData.FirstName,
        //         LastName = studentData.LastName,
        //         Age = studentData.Age,
        //         EntranceExamScore = studentData.EntranceExamScore
        //     };
        //     await data.Students.AddAsync(student);
        //     await data.SaveChangesAsync(); // Request 1
        //
        //     var groupAssignments = (studentData.Groups ?? []).Select(groupId => new GroupAssignment
        //     {
        //         GroupId = groupId,
        //         StudentId = student.Id,
        //     });
        //     await data.GroupAssignments.AddRangeAsync(groupAssignments);
        //     await data.SaveChangesAsync(); // Request 2
        //     
        //     await transaction.CommitAsync(); // Commit transaction
        // }
        // catch (Exception)
        // {
        //     await transaction.RollbackAsync(); // Rollback transaction if error occurs
        //     throw;
        // }

        // *Approach 2* - add all data via single context access
        // Map a DTO data to the student model.
        var student = new Student
        {
            FirstName = studentData.FirstName,
            LastName = studentData.LastName,
            Age = studentData.Age,
            EntranceExamScore = studentData.EntranceExamScore,
            GroupAssignments = (studentData.Groups ?? []).Select(groupId => new GroupAssignment
            {
                GroupId = groupId,
            }).ToList()
        };

        // Add new student to the db context, and save all changes.
        await data.Students.AddAsync(student);
        await data.SaveChangesAsync();


        // Return created records to the controller.
        return new StudentGetDto
        {
            Id = student.Id,
            FirstName = studentData.FirstName,
            LastName = studentData.LastName,
            Age = studentData.Age,
            EntranceExamScore = studentData.EntranceExamScore,
            Groups = groups.Select(group => new StudentGetDtoGroup
            {
                Id = group.Id,
                Name = group.Name,
            }).ToList()
        };
    }
}