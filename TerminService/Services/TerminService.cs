using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TerminService.Data;
using TherapeutKalendar.Shared.Enums;
using TherapeutKalendar.Shared.Models;
using TherapeutKalendar.Shared.Protos;

namespace TerminService.Services;

public class TerminService(ITerminRepository repository, ILogger<TerminService> logger) : TherapeutKalendar.Shared.Protos.TerminService.TerminServiceBase
{
    private readonly ITerminRepository _repository = repository;
    private readonly ILogger<TerminService> _logger = logger;

    public override async Task<TerminResponse> Get(GetRequest request, ServerCallContext context)
    {
        var start = request.From.ToDateTime();
        var end = request.To.ToDateTime();

        var termins = await _repository.GetTermineAsync(start, end);

        var response = new TerminResponse();
        response.Termins.AddRange(termins.Select(ToGrpcTerminProto));

        return response;
    }

    public override async Task<TerminProto> Create(CreateRequest request, ServerCallContext context)
    {
        if (request.StartTime == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Start time is required"));

        var startTime = request.StartTime.ToDateTime();
        var termin = new Termin
        {
            StartTime = startTime,
            EndTime = startTime.AddMinutes(30),
            Status = TerminStatus.Created,
            PatientId = Guid.Parse(request.PatientId),
            TherapistId = Guid.Parse(request.TherapistId)
        };

        var created = await _repository.CreateTerminAsync(termin);
        return ToGrpcTerminProto(created);
    }

    public override async Task<Empty> Update(UpdateRequest request, ServerCallContext context)
    {
        var success = await _repository.UpdateTerminAsync(
            Guid.Parse(request.Id),
            request.Status);

        return success ? new Empty() :
            throw new RpcException(new Status(StatusCode.NotFound, "Termin not found"));
    }

    private TerminProto ToGrpcTerminProto(Termin termin)
    {
        return new TerminProto
        {
            Id = termin.Id.ToString(),
            StartTime = Timestamp.FromDateTime(termin.StartTime.ToUniversalTime()),
            EndTime = Timestamp.FromDateTime(termin.EndTime.ToUniversalTime()),
            Status = termin.Status.ToString(),
            PatientName = $"{termin.Patient?.FirstName} {termin.Patient?.LastName}",
            TherapistName = $"{termin.Therapist?.FirstName} {termin.Therapist?.LastName}"
        };
    }
}