using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TerminService.Data;
using TherapeutKalendar.Shared.Enums;
using TherapeutKalendar.Shared.Protos;
using TherapeutKalendar.Shared.Models;

namespace TerminService.Services;

public class TherapistService : TherapeutKalendar.Shared.Protos.TherapistService.TherapistServiceBase
{
    private readonly ITherapistRepository _repository;
    private readonly ILogger<TherapistService> _logger;

    public TherapistService(ITherapistRepository repository, ILogger<TherapistService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<TherapistResponse> SearchByName(TherapistSearchByNameRequest request, ServerCallContext context)
    {
        var result = await _repository.SearchByNameAsync(request.Name);
        return new TherapistResponse { Therapists = { result.Select(ToProto) } };
    }

    public override async Task<TherapistResponse> SearchBySpecialty(TherapistSearchBySpecialtyRequest request, ServerCallContext context)
    {
        if (System.Enum.TryParse<TherapistSpecialty>(request.Specialty, out var spec))
        {
            var result = await _repository.SearchBySpecialtyAsync(spec);
            return new TherapistResponse { Therapists = { result.Select(ToProto) } };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid specialty value"));
        }
    }

    private TherapistProto ToProto(Therapist t) => new TherapistProto
    {
        Id = t.Id.ToString(),
        FirstName = t.FirstName,
        LastName = t.LastName,
        Specialty = t.Specialty.ToString(),
        DailyStartHour = t.DailyStartHour.ToString(),
        DailyEndHour = t.DailyEndHour.ToString(),
        LunchStart = t.LunchStart.ToString(),
        LunchEnd = t.LunchEnd.ToString()
    };
}