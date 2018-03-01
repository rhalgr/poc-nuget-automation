# poc-nuget-automation
Proof of concept for dynamically loading/reloading nuget packages and migrating the libraries into the app domain.

Not complete and not done by a long shot. Loading assemblies into the app domain and getting them wired up needs work. Right now the assembly doesn't attempt to resolve, and since that's not complete, the assembly resolve method is never fired.
