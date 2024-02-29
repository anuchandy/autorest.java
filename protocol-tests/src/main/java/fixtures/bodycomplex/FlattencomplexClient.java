// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.bodycomplex;

import com.azure.core.annotation.Generated;
import com.azure.core.annotation.ReturnType;
import com.azure.core.annotation.ServiceClient;
import com.azure.core.annotation.ServiceMethod;
import com.azure.core.exception.ClientAuthenticationException;
import com.azure.core.exception.HttpResponseException;
import com.azure.core.exception.ResourceModifiedException;
import com.azure.core.exception.ResourceNotFoundException;
import com.azure.core.http.rest.RequestOptions;
import com.azure.core.http.rest.Response;
import com.azure.core.util.BinaryData;
import fixtures.bodycomplex.implementation.FlattencomplexesImpl;

/**
 * Initializes a new instance of the synchronous AutoRestComplexTestServiceClient type.
 */
@ServiceClient(builder = FlattencomplexClientBuilder.class)
public final class FlattencomplexClient {
    @Generated
    private final FlattencomplexesImpl serviceClient;

    /**
     * Initializes an instance of FlattencomplexClient class.
     * 
     * @param serviceClient the service client implementation.
     */
    @Generated
    FlattencomplexClient(FlattencomplexesImpl serviceClient) {
        this.serviceClient = serviceClient;
    }

    /**
     * The getValid operation.
     * <p><strong>Response Body Schema</strong></p>
     * <pre>{@code
     * {
     *     kind: String(Kind1) (Optional)
     *     propB1: String (Optional)
     *     helper (Optional): {
     *         propBH1: String (Optional)
     *     }
     * }
     * }</pre>
     * 
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the response body along with {@link Response}.
     */
    @Generated
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Response<BinaryData> getValidWithResponse(RequestOptions requestOptions) {
        return this.serviceClient.getValidWithResponse(requestOptions);
    }
}
