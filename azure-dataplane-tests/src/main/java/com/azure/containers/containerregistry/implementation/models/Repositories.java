// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package com.azure.containers.containerregistry.implementation.models;

import com.azure.core.annotation.Fluent;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;
import java.util.List;

/**
 * List of repositories.
 */
@Fluent
public final class Repositories implements JsonSerializable<Repositories> {
    /*
     * Repository names
     */
    private List<String> repositories;

    /*
     * The link property.
     */
    private String link;

    /**
     * Creates an instance of Repositories class.
     */
    public Repositories() {
    }

    /**
     * Get the repositories property: Repository names.
     * 
     * @return the repositories value.
     */
    public List<String> getRepositories() {
        return this.repositories;
    }

    /**
     * Set the repositories property: Repository names.
     * 
     * @param repositories the repositories value to set.
     * @return the Repositories object itself.
     */
    public Repositories setRepositories(List<String> repositories) {
        this.repositories = repositories;
        return this;
    }

    /**
     * Get the link property: The link property.
     * 
     * @return the link value.
     */
    public String getLink() {
        return this.link;
    }

    /**
     * Set the link property: The link property.
     * 
     * @param link the link value to set.
     * @return the Repositories object itself.
     */
    public Repositories setLink(String link) {
        this.link = link;
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeArrayField("repositories", this.repositories, (writer, element) -> writer.writeString(element));
        jsonWriter.writeStringField("link", this.link);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of Repositories from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of Repositories if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IOException If an error occurs while reading the Repositories.
     */
    public static Repositories fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            Repositories deserializedRepositories = new Repositories();
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("repositories".equals(fieldName)) {
                    List<String> repositories = reader.readArray(reader1 -> reader1.getString());
                    deserializedRepositories.repositories = repositories;
                } else if ("link".equals(fieldName)) {
                    deserializedRepositories.link = reader.getString();
                } else {
                    reader.skipChildren();
                }
            }

            return deserializedRepositories;
        });
    }
}
