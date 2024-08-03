var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.asg_form>("asg-form");

builder.Build().Run();
