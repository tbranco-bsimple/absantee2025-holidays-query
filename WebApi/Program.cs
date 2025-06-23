using Application.DTO;
using Application.Services;
using Domain.Factory;
using Domain.IRepository;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Resolvers;
using Microsoft.EntityFrameworkCore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

//Services
builder.Services.AddTransient<HolidayPlanService>();
builder.Services.AddTransient<CollaboratorService>();
builder.Services.AddTransient<AssociationProjectCollaboratorService>();

//Repositories
builder.Services.AddTransient<IHolidayPlanRepository, HolidayPlanRepositoryEF>();
builder.Services.AddTransient<ICollaboratorRepository, CollaboratorRepositoryEF>();
builder.Services.AddTransient<IAssociationProjectCollaboratorRepository, AssociationProjectCollaboratorRepositoryEF>();


//Factories
builder.Services.AddTransient<IHolidayPlanFactory, HolidayPlanFactory>();
builder.Services.AddTransient<IHolidayPeriodFactory, HolidayPeriodFactory>();
builder.Services.AddTransient<ICollaboratorFactory, CollaboratorFactory>();
builder.Services.AddTransient<IAssociationProjectCollaboratorFactory, AssociationProjectCollaboratorFactory>();

//Mappers
builder.Services.AddTransient<HolidayPlanDataModelConverter>();
builder.Services.AddTransient<HolidayPeriodDataModelConverter>();
builder.Services.AddTransient<CollaboratorDataModelConverter>();
builder.Services.AddTransient<AssociationProjectCollaboratorDataModelConverter>();

builder.Services.AddAutoMapper(cfg =>
{
    //DataModels
    cfg.AddProfile<DataModelMappingProfile>();

    //DTO
    cfg.CreateMap<HolidayPlan, HolidayPlanDTO>();
    cfg.CreateMap<HolidayPlanDTO, HolidayPlan>();
    cfg.CreateMap<HolidayPeriod, HolidayPeriodDTO>();
    cfg.CreateMap<HolidayPeriodDTO, HolidayPeriod>();
    cfg.CreateMap<HolidayPeriod, CreateHolidayPeriodDTO>()
            .ForMember(dest => dest.InitDate, opt => opt.MapFrom(src => src.PeriodDate.InitDate))
            .ForMember(dest => dest.FinalDate, opt => opt.MapFrom(src => src.PeriodDate.FinalDate));
    cfg.CreateMap<Collaborator, CollaboratorDTO>();
    cfg.CreateMap<CollaboratorDTO, Collaborator>();
    cfg.CreateMap<AssociationProjectCollaborator, AssociationProjectCollaboratorDTO>();
    cfg.CreateMap<AssociationProjectCollaboratorDTO, AssociationProjectCollaborator>();
});

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HolidayPeriodConsumer>();
    x.AddConsumer<HolidayPlanConsumer>();
    x.AddConsumer<CollaboratorConsumer>();
    x.AddConsumer<AssociationProjectCollaboratorConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("holidays-query", e =>
        {
            e.ConfigureConsumer<HolidayPeriodConsumer>(context);
            e.ConfigureConsumer<HolidayPlanConsumer>(context);
            e.ConfigureConsumer<CollaboratorConsumer>(context);
            e.ConfigureConsumer<AssociationProjectCollaboratorConsumer>(context);
        });
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
