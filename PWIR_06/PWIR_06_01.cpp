#include <cstdio>
#include <cstdint>
#include <cstdlib>
#include <chrono>
#include <assert.h>
#include <windows.h>
#include <omp.h>

void wait(int x){
    Sleep(x);
}

int main(){
    int32_t i;
    uint32_t n = 10;
    auto start = std::chrono::high_resolution_clock::now();
    #pragma omp parallel num_threads(2) default(none) shared(n)
    {
        #pragma omp sections nowait
        {
            #pragma omp section
            {
                printf("Sections - Thread %d working...\n", omp_get_thread_num());
                wait(2000);
            }

            #pragma omp section
            {
                printf("Sections - Thread %d working ...\n", omp_get_thread_num());
                wait(4000);
            }
        }

        #pragma omp for schedule(dynamic) private(i)
        for (i = 0; i < n; i++) {
            printf("Iteration %d execute thread %d.\n", i, omp_get_thread_num());
            wait(400);
        }
    }

    auto end = std::chrono::high_resolution_clock::now();
    printf("Parallel normal way %llu ms\r\n",
    std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count());

	getchar();
    return 0;
}


//Sections - Thread 0 working...
//Sections - Thread 1 working ...
//Iteration 0 execute thread 0.
//Iteration 1 execute thread 0.
//Iteration 2 execute thread 0.
//Iteration 3 execute thread 0.
//Iteration 4 execute thread 0.
//Iteration 5 execute thread 1.
//Iteration 6 execute thread 0.
//Iteration 7 execute thread 1.
//Iteration 8 execute thread 0.
//Iteration 9 execute thread 1.
//Parallel normal way 5216 ms