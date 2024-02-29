// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.constants.models;

import com.azure.core.annotation.Fluent;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;

/**
 * The NoModelAsStringRequiredOneValueDefault model.
 */
@Fluent
public final class NoModelAsStringRequiredOneValueDefault
    implements JsonSerializable<NoModelAsStringRequiredOneValueDefault> {
    /*
     * The parameter property.
     */
    private String parameter = "value1";

    /**
     * Creates an instance of NoModelAsStringRequiredOneValueDefault class.
     */
    public NoModelAsStringRequiredOneValueDefault() {
    }

    /**
     * Get the parameter property: The parameter property.
     * 
     * @return the parameter value.
     */
    public String getParameter() {
        return this.parameter;
    }

    /**
     * Set the parameter property: The parameter property.
     * 
     * @param parameter the parameter value to set.
     * @return the NoModelAsStringRequiredOneValueDefault object itself.
     */
    public NoModelAsStringRequiredOneValueDefault setParameter(String parameter) {
        this.parameter = parameter;
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
        jsonWriter.writeStringField("parameter", this.parameter);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of NoModelAsStringRequiredOneValueDefault from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of NoModelAsStringRequiredOneValueDefault if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IllegalStateException If the deserialized JSON object was missing any required properties.
     * @throws IOException If an error occurs while reading the NoModelAsStringRequiredOneValueDefault.
     */
    public static NoModelAsStringRequiredOneValueDefault fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            NoModelAsStringRequiredOneValueDefault deserializedNoModelAsStringRequiredOneValueDefault
                = new NoModelAsStringRequiredOneValueDefault();
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                reader.skipChildren();
            }

            return deserializedNoModelAsStringRequiredOneValueDefault;
        });
    }
}
