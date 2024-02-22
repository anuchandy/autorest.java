// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) TypeSpec Code Generator.

package com._specs_.azure.clientgenerator.core.access.implementation.models;

import com.azure.core.annotation.Generated;
import com.azure.core.annotation.Immutable;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;

/**
 * Used in internal operations, should be generated but not exported.
 */
@Immutable
public class AbstractModel implements JsonSerializable<AbstractModel> {
    /*
     * The kind property.
     */
    @Generated
    private String kind;

    /*
     * The name property.
     */
    @Generated
    private final String name;

    /**
     * Creates an instance of AbstractModel class.
     * 
     * @param name the name value to set.
     */
    @Generated
    protected AbstractModel(String name) {
        this.kind = "AbstractModel";
        this.name = name;
    }

    /**
     * Get the kind property: The kind property.
     * 
     * @return the kind value.
     */
    @Generated
    public String getKind() {
        return this.kind;
    }

    /**
     * Set the kind property: The kind property.
     * 
     * @param kind the kind value to set.
     * @return the AbstractModel object itself.
     */
    @Generated
    AbstractModel setKind(String kind) {
        this.kind = kind;
        return this;
    }

    /**
     * Get the name property: The name property.
     * 
     * @return the name value.
     */
    @Generated
    public String getName() {
        return this.name;
    }

    @Generated
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeStringField("name", this.name);
        jsonWriter.writeStringField("kind", this.kind);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of AbstractModel from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of AbstractModel if the JsonReader was pointing to an instance of it, or null if it was
     * pointing to JSON null.
     * @throws IllegalStateException If the deserialized JSON object was missing any required properties.
     * @throws IOException If an error occurs while reading the AbstractModel.
     */
    @Generated
    public static AbstractModel fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            String discriminatorValue = null;
            try (JsonReader readerToUse = reader.bufferObject()) {
                readerToUse.nextToken(); // Prepare for reading
                while (readerToUse.nextToken() != JsonToken.END_OBJECT) {
                    String fieldName = readerToUse.getFieldName();
                    readerToUse.nextToken();
                    if ("kind".equals(fieldName)) {
                        discriminatorValue = readerToUse.getString();
                        break;
                    } else {
                        readerToUse.skipChildren();
                    }
                }
                // Use the discriminator value to determine which subtype should be deserialized.
                if ("real".equals(discriminatorValue)) {
                    return RealModel.fromJson(readerToUse.reset());
                } else {
                    return fromJsonKnownDiscriminator(readerToUse.reset());
                }
            }
        });
    }

    @Generated
    static AbstractModel fromJsonKnownDiscriminator(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            String name = null;
            String kind = null;
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("name".equals(fieldName)) {
                    name = reader.getString();
                } else if ("kind".equals(fieldName)) {
                    kind = reader.getString();
                } else {
                    reader.skipChildren();
                }
            }
            AbstractModel deserializedAbstractModel = new AbstractModel(name);
            deserializedAbstractModel.kind = kind;

            return deserializedAbstractModel;
        });
    }
}
