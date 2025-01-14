var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CloudPanel_WebApi>("cloudpanel");

builder.AddProject<Projects.CloudPanel_WebApp>("webapp");

builder.Build().Run();
