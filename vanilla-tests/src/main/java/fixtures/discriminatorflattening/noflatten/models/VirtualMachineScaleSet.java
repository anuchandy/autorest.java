// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.discriminatorflattening.noflatten.models;

import com.azure.core.annotation.Fluent;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;

/**
 * Describes a Virtual Machine Scale Set.
 */
@Fluent
public final class VirtualMachineScaleSet implements JsonSerializable<VirtualMachineScaleSet> {
    /*
     * Describes the properties of a Virtual Machine Scale Set.
     */
    private VirtualMachineScaleSetProperties properties;

    /**
     * Creates an instance of VirtualMachineScaleSet class.
     */
    public VirtualMachineScaleSet() {
    }

    /**
     * Get the properties property: Describes the properties of a Virtual Machine Scale Set.
     * 
     * @return the properties value.
     */
    public VirtualMachineScaleSetProperties getProperties() {
        return this.properties;
    }

    /**
     * Set the properties property: Describes the properties of a Virtual Machine Scale Set.
     * 
     * @param properties the properties value to set.
     * @return the VirtualMachineScaleSet object itself.
     */
    public VirtualMachineScaleSet setProperties(VirtualMachineScaleSetProperties properties) {
        this.properties = properties;
        return this;
    }

    /**
     * Validates the instance.
     * 
     * @throws IllegalArgumentException thrown if the instance is not valid.
     */
    public void validate() {
        if (getProperties() != null) {
            getProperties().validate();
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeJsonField("properties", this.properties);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of VirtualMachineScaleSet from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of VirtualMachineScaleSet if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IOException If an error occurs while reading the VirtualMachineScaleSet.
     */
    public static VirtualMachineScaleSet fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            VirtualMachineScaleSet deserializedVirtualMachineScaleSet = new VirtualMachineScaleSet();
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("properties".equals(fieldName)) {
                    deserializedVirtualMachineScaleSet.properties = VirtualMachineScaleSetProperties.fromJson(reader);
                } else {
                    reader.skipChildren();
                }
            }

            return deserializedVirtualMachineScaleSet;
        });
    }
}
