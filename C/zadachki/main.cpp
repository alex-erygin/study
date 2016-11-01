#include <gtest/gtest.h>
#include "src/BitCounter.h"

TEST(BasicTests, TestName) {
    BitCounter bitCounter;

    int testCaseCount = 7;
    int testCases[testCaseCount][2] = {
            { 0, 0 },
            { 1, 1 },
            { 1, 2 },
            { 2, 3 },
            { 1, 4 },
            { 2, 5 },
            { 2, 6 },
    };

    for (int i = 0; i < testCaseCount; ++i) {
        int result = bitCounter.CountBits(testCases[i][1]);
        ASSERT_EQ(testCases[i][0], result);
    }

}
