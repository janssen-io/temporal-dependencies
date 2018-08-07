namespace Temporal.Core

module Domain =
    type Filename = Filename of string
    type Dependency = Filename * Filename
    type Dependencies = Map<Dependency, int>

    let addCount (dep,count) deps =
        if Map.containsKey dep deps then
            Map.add dep (deps.[dep] + count) deps
        else
            Map.add dep count deps

    let addDependency dep = addCount (dep, 1)

    let mergeDependencies source destination =
        let addFolder deps dep count = addCount (dep,count) deps
        Map.fold addFolder source destination
        