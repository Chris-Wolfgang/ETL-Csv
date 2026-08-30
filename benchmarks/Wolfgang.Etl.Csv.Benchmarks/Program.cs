using BenchmarkDotNet.Running;
// These files still configure via the deprecated property setters in places where the value is
// applied after construction, so it cannot travel through the options constructor without
// restructuring the test. They keep exercising the setter path until the setters are removed.
#pragma warning disable CS0618


BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);
