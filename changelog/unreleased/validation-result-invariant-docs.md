type: docs

`CsvValidationResult` documentation now states that its invariants are enforced by the constructors only: the public `init` accessors on `IsValid` and `Failures` (shipped API) still allow `with` / initializer expressions to create states the constructors reject.
