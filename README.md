# record-search

1. Clone the repository

    ```bash
    git clone https://github.com/rferstl/record-search.git
    ```

2. Run "Developer Command Prompt for VS 2019"

3. Build the library project:

    ```bash
    cd JaroWinklerRecordSearch
    msbuild JaroWinklerRecordSearch.csproj /t:rebuild /property:Configuration=Debug
    ```

4. Build the test project:

    ```bash
    cd JaroWinklerRecordSearch.Test
    msbuild JaroWinklerRecordSearch.Test.csproj /t:rebuild /property:Configuration=Debug
    ```

5. run test:

    ```bash
    cd JaroWinklerRecordSearch.Test/bin/Debug
    JaroWinklerRecordSearch.Test.exe
    ```


Based on:

##### 1. Jaro-Winkler similarity
 * https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
 * https://www.geeksforgeeks.org/jaro-and-jaro-winkler-similarity/

##### 2. Fast Fuzzy String Matching - Seth Verrinder & Kyle Putnam - Midwest.io 2015
 * https://www.youtube.com/watch?v=s0YSKiFdj8Q

##### 3. Hungarian method, Kuhn–Munkres algorithm or Munkres assignment algorithm
 * https://en.wikipedia.org/wiki/Hungarian_algorithm
 * https://github.com/accord-net/framework/blob/development/Sources/Accord.Math/Optimization/Munkres.cs
 * http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html


