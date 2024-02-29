// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.nonamedresponsetypes.implementation;

import com.azure.core.models.ResponseError;
import com.azure.json.JsonReader;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;

import java.io.IOException;
import java.time.Duration;

/**
 * Utility class that handles functionality not yet available in azure-core that generated code requires.
 */
public final class CoreToCodegenBridgeUtils {
    /**
     * Writes the object to the passed {@link ResponseError}.
     *
     * @param jsonWriter Where the {@link ResponseError} JSON will be written.
     * @return The {@link JsonWriter} where the JSON was written.
     * @throws IOException If the {@link ResponseError} fails to be written to the {@code jsonWriter}.
     */
    public static JsonWriter responseErrorToJson(JsonWriter jsonWriter, ResponseError responseError)
        throws IOException {
        return jsonWriter.writeStartObject()
            .writeStringField("code", responseError.getCode())
            .writeStringField("message", responseError.getMessage())
            .writeEndObject();
    }

    /**
     * Reads a JSON stream into a {@link ResponseError}.
     *
     * @param jsonReader The {@link JsonReader} being read.
     * @return The {@link ResponseError} that the JSON stream represented, or null if it pointed to JSON null.
     * @throws IllegalStateException If the deserialized JSON object was missing any required properties.
     * @throws IOException If a {@link ResponseError} fails to be read from the {@code jsonReader}.
     */
    public static ResponseError responseErrorFromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            // Buffer the next JSON object as ResponseError can take two forms:
            //
            // - A ResponseError object
            // - A ResponseError object wrapped in an "error" node.
            JsonReader bufferedReader = reader.bufferObject();
            bufferedReader.nextToken(); // Get to the START_OBJECT token.
            while (bufferedReader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = bufferedReader.getFieldName();
                bufferedReader.nextToken();

                if ("error".equals(fieldName)) {
                    // If the ResponseError was wrapped in the "error" node begin reading it now.
                    return readResponseError(bufferedReader);
                } else {
                    bufferedReader.skipChildren();
                }
            }

            // Otherwise reset the JsonReader and read the whole JSON object.
            return readResponseError(bufferedReader.reset());
        });
    }

    private static ResponseError readResponseError(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            String code = null;
            boolean codeFound = false;
            String message = null;
            boolean messageFound = false;

            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("code".equals(fieldName)) {
                    code = reader.getString();
                    codeFound = true;
                } else if ("message".equals(fieldName)) {
                    message = reader.getString();
                    messageFound = true;
                } else {
                    reader.skipChildren();
                }
            }

            if (!codeFound && !messageFound) {
                throw new IllegalStateException("Missing required properties: code, message");
            } else if (!codeFound) {
                throw new IllegalStateException("Missing required property: code");
            } else if (!messageFound) {
                throw new IllegalStateException("Missing required property: message");
            }

            return new ResponseError(code, message);
        });
    }

    /**
     * Converts a {@link Duration} to a string in ISO-8601 format with support for a day component.
     *
     * @param duration The {@link Duration} to convert.
     * @return The {@link Duration} as a string in ISO-8601 format with support for a day component, or null if the
     * provided {@link Duration} was null.
     */
    public static String durationToStringWithDays(Duration duration) {
        if (duration == null) {
            return null;
        }

        if (duration.isZero()) {
            return "PT0S";
        }

        StringBuilder builder = new StringBuilder();

        if (duration.isNegative()) {
            builder.append("-P");
            duration = duration.negated();
        } else {
            builder.append('P');
        }

        long days = duration.toDays();
        if (days > 0) {
            builder.append(days);
            builder.append('D');
            duration = duration.minusDays(days);
        }

        long hours = duration.toHours();
        if (hours > 0) {
            builder.append('T');
            builder.append(hours);
            builder.append('H');
            duration = duration.minusHours(hours);
        }

        final long minutes = duration.toMinutes();
        if (minutes > 0) {
            if (hours == 0) {
                builder.append('T');
            }

            builder.append(minutes);
            builder.append('M');
            duration = duration.minusMinutes(minutes);
        }

        final long seconds = duration.getSeconds();
        if (seconds > 0) {
            if (hours == 0 && minutes == 0) {
                builder.append('T');
            }

            builder.append(seconds);
            duration = duration.minusSeconds(seconds);
        }

        long milliseconds = duration.toMillis();
        if (milliseconds > 0) {
            if (hours == 0 && minutes == 0 && seconds == 0) {
                builder.append("T");
            }

            if (seconds == 0) {
                builder.append("0");
            }

            builder.append('.');

            if (milliseconds <= 99) {
                builder.append('0');

                if (milliseconds <= 9) {
                    builder.append('0');
                }
            }

            // Remove trailing zeros.
            while (milliseconds % 10 == 0) {
                milliseconds /= 10;
            }
            builder.append(milliseconds);
        }

        if (seconds > 0 || milliseconds > 0) {
            builder.append('S');
        }

        return builder.toString();
    }
}
