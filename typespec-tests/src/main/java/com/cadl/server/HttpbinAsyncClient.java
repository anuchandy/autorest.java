// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package com.cadl.server;

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
import com.azure.core.util.FluxUtil;
import com.cadl.server.implementation.HttpbinClientImpl;
import reactor.core.publisher.Mono;

/** Initializes a new instance of the asynchronous HttpbinClient type. */
@ServiceClient(builder = HttpbinClientBuilder.class, isAsync = true)
public final class HttpbinAsyncClient {
    @Generated private final HttpbinClientImpl serviceClient;

    /**
     * Initializes an instance of HttpbinAsyncClient class.
     *
     * @param serviceClient the service client implementation.
     */
    @Generated
    HttpbinAsyncClient(HttpbinClientImpl serviceClient) {
        this.serviceClient = serviceClient;
    }

    /**
     * The status operation.
     *
     * @param code A 32-bit integer. (`-2,147,483,648` to `2,147,483,647`).
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link Response} on successful completion of {@link Mono}.
     */
    @Generated
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Mono<Response<Void>> statusWithResponse(int code, RequestOptions requestOptions) {
        return this.serviceClient.statusWithResponseAsync(code, requestOptions);
    }

    /**
     * The status operation.
     *
     * @param code A 32-bit integer. (`-2,147,483,648` to `2,147,483,647`).
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return A {@link Mono} that completes when a successful response is received.
     */
    @Generated
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Mono<Void> status(int code) {
        // Generated convenience method for statusWithResponse
        RequestOptions requestOptions = new RequestOptions();
        return statusWithResponse(code, requestOptions).flatMap(FluxUtil::toMono);
    }
}
