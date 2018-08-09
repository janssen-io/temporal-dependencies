# Temporal Dependencies
This tool computes temporal dependencies in a version controlled repository. A pair is considered coupled when they occur in the same commit. Plenty versions of this type of tool already exist, I just created another one to learn F\#.

# Structure
There are three projects in the `src/` directory. The `tests/` folder has a similar structure with tests for the respective projects.

## Temporal.Console
This project provides a command line utility to compute the dependencies based on a logfile or the output of `tf` or `git`. 
Several arguments can be provided to filter the output:

* vcs: which version control system to use (git/tfs). 
* file: read data from file.
* ignore: comma separated list of extensions/files to ignore.
* min n: only show the dependencies with at least _n_ occurences.
* top n: only show the top _n_ dependencies.

## Temporal.Core.Input
This project contains all necessary logic to parse the output of `git log --name-only --format="new commit"` or `tf hist \recursive \format:detailed`. For git, it assumes that the commits start with 'new commit' followed by the changed filenames on separate lines. For TFS, the detailed format is assumed.

## Temporal.Core.Domain
This project contains all the logic to compute the actual dependencies.
