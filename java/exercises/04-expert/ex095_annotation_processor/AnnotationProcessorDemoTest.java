package fewolearning.exercises.expert.ex095_annotation_processor;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class AnnotationProcessorDemoTest {

    @Test
    void namesFollowingTheConventionProduceNoViolations() {
        List<String> names = List.of("GeneratedUserDto", "GeneratedOrderMapper");

        assertEquals(List.of(), AnnotationProcessorDemo.validateNames(names));
    }

    @Test
    void namesThatDoNotStartWithGeneratedAreReportedAsViolations() {
        List<String> names = List.of("UserDto", "generatedOrderMapper");

        assertEquals(List.of("UserDto", "generatedOrderMapper"), AnnotationProcessorDemo.validateNames(names));
    }

    @Test
    void namesMissingTheRequiredUppercaseLetterAfterGeneratedAreViolations() {
        List<String> names = List.of("Generated", "Generated1Dto");

        assertEquals(List.of("Generated", "Generated1Dto"), AnnotationProcessorDemo.validateNames(names));
    }

    @Test
    void mixedCompliantAndViolatingNamesReturnOnlyTheViolations() {
        List<String> names = List.of("GeneratedUserDto", "userDto", "GeneratedOrderMapper", "OrderMapper");

        assertEquals(List.of("userDto", "OrderMapper"), AnnotationProcessorDemo.validateNames(names));
    }
}
