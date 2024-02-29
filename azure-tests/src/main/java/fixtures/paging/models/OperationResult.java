// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.paging.models;

import com.azure.core.annotation.Fluent;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;

/**
 * The OperationResult model.
 */
@Fluent
public final class OperationResult implements JsonSerializable<OperationResult> {
    /*
     * The status of the request
     */
    private OperationResultStatus status;

    /**
     * Creates an instance of OperationResult class.
     */
    public OperationResult() {
    }

    /**
     * Get the status property: The status of the request.
     * 
     * @return the status value.
     */
    public OperationResultStatus getStatus() {
        return this.status;
    }

    /**
     * Set the status property: The status of the request.
     * 
     * @param status the status value to set.
     * @return the OperationResult object itself.
     */
    public OperationResult setStatus(OperationResultStatus status) {
        this.status = status;
        return this;
    }

    /**
     * Validates the instance.
     * 
     * @throws IllegalArgumentException thrown if the instance is not valid.
     */
    public void validate() {
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeStringField("status", this.status == null ? null : this.status.toString());
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of OperationResult from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of OperationResult if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IOException If an error occurs while reading the OperationResult.
     */
    public static OperationResult fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            OperationResult deserializedOperationResult = new OperationResult();
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("status".equals(fieldName)) {
                    deserializedOperationResult.status = OperationResultStatus.fromString(reader.getString());
                } else {
                    reader.skipChildren();
                }
            }

            return deserializedOperationResult;
        });
    }
}
