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
 * Parameter group.
 */
@Fluent
public final class PagingGetMultiplePagesLroOptions implements JsonSerializable<PagingGetMultiplePagesLroOptions> {
    /*
     * Sets the maximum number of items to return in the response.
     */
    private Integer maxresults;

    /*
     * Sets the maximum time that the server can spend processing the request, in seconds. The default is 30 seconds.
     */
    private Integer timeout;

    /**
     * Creates an instance of PagingGetMultiplePagesLroOptions class.
     */
    public PagingGetMultiplePagesLroOptions() {
    }

    /**
     * Get the maxresults property: Sets the maximum number of items to return in the response.
     * 
     * @return the maxresults value.
     */
    public Integer getMaxresults() {
        return this.maxresults;
    }

    /**
     * Set the maxresults property: Sets the maximum number of items to return in the response.
     * 
     * @param maxresults the maxresults value to set.
     * @return the PagingGetMultiplePagesLroOptions object itself.
     */
    public PagingGetMultiplePagesLroOptions setMaxresults(Integer maxresults) {
        this.maxresults = maxresults;
        return this;
    }

    /**
     * Get the timeout property: Sets the maximum time that the server can spend processing the request, in seconds. The default is 30 seconds.
     * 
     * @return the timeout value.
     */
    public Integer getTimeout() {
        return this.timeout;
    }

    /**
     * Set the timeout property: Sets the maximum time that the server can spend processing the request, in seconds. The default is 30 seconds.
     * 
     * @param timeout the timeout value to set.
     * @return the PagingGetMultiplePagesLroOptions object itself.
     */
    public PagingGetMultiplePagesLroOptions setTimeout(Integer timeout) {
        this.timeout = timeout;
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
        jsonWriter.writeNumberField("maxresults", this.maxresults);
        jsonWriter.writeNumberField("timeout", this.timeout);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of PagingGetMultiplePagesLroOptions from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of PagingGetMultiplePagesLroOptions if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IOException If an error occurs while reading the PagingGetMultiplePagesLroOptions.
     */
    public static PagingGetMultiplePagesLroOptions fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            PagingGetMultiplePagesLroOptions deserializedPagingGetMultiplePagesLroOptions
                = new PagingGetMultiplePagesLroOptions();
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("maxresults".equals(fieldName)) {
                    deserializedPagingGetMultiplePagesLroOptions.maxresults = reader.getNullable(JsonReader::getInt);
                } else if ("timeout".equals(fieldName)) {
                    deserializedPagingGetMultiplePagesLroOptions.timeout = reader.getNullable(JsonReader::getInt);
                } else {
                    reader.skipChildren();
                }
            }

            return deserializedPagingGetMultiplePagesLroOptions;
        });
    }
}
