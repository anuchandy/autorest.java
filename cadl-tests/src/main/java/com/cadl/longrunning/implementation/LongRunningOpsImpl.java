// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package com.cadl.longrunning.implementation;

import com.azure.core.annotation.BodyParam;
import com.azure.core.annotation.Delete;
import com.azure.core.annotation.ExpectedResponses;
import com.azure.core.annotation.Get;
import com.azure.core.annotation.HeaderParam;
import com.azure.core.annotation.Host;
import com.azure.core.annotation.HostParam;
import com.azure.core.annotation.Patch;
import com.azure.core.annotation.PathParam;
import com.azure.core.annotation.Post;
import com.azure.core.annotation.QueryParam;
import com.azure.core.annotation.ReturnType;
import com.azure.core.annotation.ServiceInterface;
import com.azure.core.annotation.ServiceMethod;
import com.azure.core.annotation.UnexpectedResponseExceptionType;
import com.azure.core.exception.ClientAuthenticationException;
import com.azure.core.exception.HttpResponseException;
import com.azure.core.exception.ResourceModifiedException;
import com.azure.core.exception.ResourceNotFoundException;
import com.azure.core.http.rest.RequestOptions;
import com.azure.core.http.rest.Response;
import com.azure.core.http.rest.RestProxy;
import com.azure.core.util.BinaryData;
import com.azure.core.util.Context;
import com.azure.core.util.FluxUtil;
import com.azure.core.util.polling.DefaultPollingStrategy;
import com.azure.core.util.polling.PollerFlux;
import com.azure.core.util.polling.SyncPoller;
import com.azure.core.util.serializer.TypeReference;
import java.time.Duration;
import reactor.core.publisher.Mono;

/** An instance of this class provides access to all the operations defined in LongRunningOps. */
public final class LongRunningOpsImpl {
    /** The proxy service used to perform REST calls. */
    private final LongRunningOpsService service;

    /** The service client containing this operation class. */
    private final LongRunningClientImpl client;

    /**
     * Initializes an instance of LongRunningOpsImpl.
     *
     * @param client the instance of the service client containing this operation class.
     */
    LongRunningOpsImpl(LongRunningClientImpl client) {
        this.service =
                RestProxy.create(LongRunningOpsService.class, client.getHttpPipeline(), client.getSerializerAdapter());
        this.client = client;
    }

    /**
     * The interface defining all the services for LongRunningLongRunningOps to be used by the proxy service to perform
     * REST calls.
     */
    @Host("{endpoint}")
    @ServiceInterface(name = "LongRunningLongRunni")
    private interface LongRunningOpsService {
        @Patch("/long-running/{name}")
        @ExpectedResponses({200, 201})
        @UnexpectedResponseExceptionType(
                value = ClientAuthenticationException.class,
                code = {401})
        @UnexpectedResponseExceptionType(
                value = ResourceNotFoundException.class,
                code = {404})
        @UnexpectedResponseExceptionType(
                value = ResourceModifiedException.class,
                code = {409})
        @UnexpectedResponseExceptionType(HttpResponseException.class)
        Mono<Response<BinaryData>> createOrUpdate(
                @HostParam("endpoint") String endpoint,
                @PathParam("name") String name,
                @HeaderParam("content-type") String contentType,
                @QueryParam("api-version") String apiVersion,
                @HeaderParam("accept") String accept,
                @BodyParam("application/merge-patch+json") BinaryData optionalProperties,
                RequestOptions requestOptions,
                Context context);

        @Get("/long-running/{name}")
        @ExpectedResponses({200})
        @UnexpectedResponseExceptionType(
                value = ClientAuthenticationException.class,
                code = {401})
        @UnexpectedResponseExceptionType(
                value = ResourceNotFoundException.class,
                code = {404})
        @UnexpectedResponseExceptionType(
                value = ResourceModifiedException.class,
                code = {409})
        @UnexpectedResponseExceptionType(HttpResponseException.class)
        Mono<Response<BinaryData>> get(
                @HostParam("endpoint") String endpoint,
                @PathParam("name") String name,
                @QueryParam("api-version") String apiVersion,
                @HeaderParam("accept") String accept,
                RequestOptions requestOptions,
                Context context);

        @Delete("/long-running/{name}")
        @ExpectedResponses({202})
        @UnexpectedResponseExceptionType(
                value = ClientAuthenticationException.class,
                code = {401})
        @UnexpectedResponseExceptionType(
                value = ResourceNotFoundException.class,
                code = {404})
        @UnexpectedResponseExceptionType(
                value = ResourceModifiedException.class,
                code = {409})
        @UnexpectedResponseExceptionType(HttpResponseException.class)
        Mono<Response<BinaryData>> delete(
                @HostParam("endpoint") String endpoint,
                @PathParam("name") String name,
                @QueryParam("api-version") String apiVersion,
                @HeaderParam("accept") String accept,
                RequestOptions requestOptions,
                Context context);

        @Post("/long-running/{name}:export")
        @ExpectedResponses({202})
        @UnexpectedResponseExceptionType(
                value = ClientAuthenticationException.class,
                code = {401})
        @UnexpectedResponseExceptionType(
                value = ResourceNotFoundException.class,
                code = {404})
        @UnexpectedResponseExceptionType(
                value = ResourceModifiedException.class,
                code = {409})
        @UnexpectedResponseExceptionType(HttpResponseException.class)
        Mono<Response<Void>> export(
                @HostParam("endpoint") String endpoint,
                @PathParam("name") String name,
                @QueryParam("projectFileVersion") String projectFileVersion,
                @QueryParam("api-version") String apiVersion,
                @HeaderParam("accept") String accept,
                RequestOptions requestOptions,
                Context context);
    }

    /**
     * Creates or updates a Resource asynchronously.
     *
     * <p><strong>Request Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     type: String (Optional)
     * }
     * }</pre>
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     name: String (Required)
     *     type: String (Required)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param optionalProperties The template for adding optional properties.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the response body along with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<BinaryData>> createOrUpdateWithResponseAsync(
            String name, BinaryData optionalProperties, RequestOptions requestOptions) {
        final String contentType = "application/merge-patch+json";
        final String accept = "application/json";
        return FluxUtil.withContext(
                context ->
                        service.createOrUpdate(
                                this.client.getEndpoint(),
                                name,
                                contentType,
                                this.client.getServiceVersion().getVersion(),
                                accept,
                                optionalProperties,
                                requestOptions,
                                context));
    }

    /**
     * Creates or updates a Resource asynchronously.
     *
     * <p><strong>Request Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     type: String (Optional)
     * }
     * }</pre>
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     name: String (Required)
     *     type: String (Required)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param optionalProperties The template for adding optional properties.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link PollerFlux} for polling of long-running operation.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public PollerFlux<BinaryData, BinaryData> beginCreateOrUpdateAsync(
            String name, BinaryData optionalProperties, RequestOptions requestOptions) {
        return PollerFlux.create(
                Duration.ofSeconds(1),
                () -> this.createOrUpdateWithResponseAsync(name, optionalProperties, requestOptions),
                new DefaultPollingStrategy<>(
                        this.client.getHttpPipeline(),
                        null,
                        requestOptions != null && requestOptions.getContext() != null
                                ? requestOptions.getContext()
                                : Context.NONE),
                TypeReference.createInstance(BinaryData.class),
                TypeReference.createInstance(BinaryData.class));
    }

    /**
     * Creates or updates a Resource asynchronously.
     *
     * <p><strong>Request Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     type: String (Optional)
     * }
     * }</pre>
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     name: String (Required)
     *     type: String (Required)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param optionalProperties The template for adding optional properties.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link SyncPoller} for polling of long-running operation.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public SyncPoller<BinaryData, BinaryData> beginCreateOrUpdate(
            String name, BinaryData optionalProperties, RequestOptions requestOptions) {
        return this.beginCreateOrUpdateAsync(name, optionalProperties, requestOptions).getSyncPoller();
    }

    /**
     * Get a Resource.
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     name: String (Required)
     *     type: String (Required)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return a Resource along with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Mono<Response<BinaryData>> getWithResponseAsync(String name, RequestOptions requestOptions) {
        final String accept = "application/json";
        return FluxUtil.withContext(
                context ->
                        service.get(
                                this.client.getEndpoint(),
                                name,
                                this.client.getServiceVersion().getVersion(),
                                accept,
                                requestOptions,
                                context));
    }

    /**
     * Get a Resource.
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     name: String (Required)
     *     type: String (Required)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return a Resource along with {@link Response}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Response<BinaryData> getWithResponse(String name, RequestOptions requestOptions) {
        return getWithResponseAsync(name, requestOptions).block();
    }

    /**
     * Delete a Resource asynchronously.
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     status: String(InProgress/Succeeded/Failed/Canceled) (Required)
     *     error: ResponseError (Optional)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return status monitor resource for long running operations along with {@link Response} on successful completion
     *     of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<BinaryData>> deleteWithResponseAsync(String name, RequestOptions requestOptions) {
        final String accept = "application/json";
        return FluxUtil.withContext(
                context ->
                        service.delete(
                                this.client.getEndpoint(),
                                name,
                                this.client.getServiceVersion().getVersion(),
                                accept,
                                requestOptions,
                                context));
    }

    /**
     * Delete a Resource asynchronously.
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     status: String(InProgress/Succeeded/Failed/Canceled) (Required)
     *     error: ResponseError (Optional)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link PollerFlux} for polling of status monitor resource for long running operations.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public PollerFlux<BinaryData, BinaryData> beginDeleteAsync(String name, RequestOptions requestOptions) {
        return PollerFlux.create(
                Duration.ofSeconds(1),
                () -> this.deleteWithResponseAsync(name, requestOptions),
                new DefaultPollingStrategy<>(
                        this.client.getHttpPipeline(),
                        null,
                        requestOptions != null && requestOptions.getContext() != null
                                ? requestOptions.getContext()
                                : Context.NONE),
                TypeReference.createInstance(BinaryData.class),
                TypeReference.createInstance(BinaryData.class));
    }

    /**
     * Delete a Resource asynchronously.
     *
     * <p><strong>Response Body Schema</strong>
     *
     * <pre>{@code
     * {
     *     id: String (Required)
     *     status: String(InProgress/Succeeded/Failed/Canceled) (Required)
     *     error: ResponseError (Optional)
     * }
     * }</pre>
     *
     * @param name The name parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link SyncPoller} for polling of status monitor resource for long running operations.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public SyncPoller<BinaryData, BinaryData> beginDelete(String name, RequestOptions requestOptions) {
        return this.beginDeleteAsync(name, requestOptions).getSyncPoller();
    }

    /**
     * Runs a custom action on Resource.
     *
     * @param name The name parameter.
     * @param projectFileVersion The projectFileVersion parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<Void>> exportWithResponseAsync(
            String name, String projectFileVersion, RequestOptions requestOptions) {
        final String accept = "application/json";
        return FluxUtil.withContext(
                context ->
                        service.export(
                                this.client.getEndpoint(),
                                name,
                                projectFileVersion,
                                this.client.getServiceVersion().getVersion(),
                                accept,
                                requestOptions,
                                context));
    }

    /**
     * Runs a custom action on Resource.
     *
     * @param name The name parameter.
     * @param projectFileVersion The projectFileVersion parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link PollerFlux} for polling of long-running operation.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public PollerFlux<BinaryData, BinaryData> beginExportAsync(
            String name, String projectFileVersion, RequestOptions requestOptions) {
        return PollerFlux.create(
                Duration.ofSeconds(1),
                () -> this.exportWithResponseAsync(name, projectFileVersion, requestOptions),
                new DefaultPollingStrategy<>(
                        this.client.getHttpPipeline(),
                        null,
                        requestOptions != null && requestOptions.getContext() != null
                                ? requestOptions.getContext()
                                : Context.NONE),
                TypeReference.createInstance(BinaryData.class),
                TypeReference.createInstance(BinaryData.class));
    }

    /**
     * Runs a custom action on Resource.
     *
     * @param name The name parameter.
     * @param projectFileVersion The projectFileVersion parameter.
     * @param requestOptions The options to configure the HTTP request before HTTP client sends it.
     * @throws HttpResponseException thrown if the request is rejected by server.
     * @throws ClientAuthenticationException thrown if the request is rejected by server on status code 401.
     * @throws ResourceNotFoundException thrown if the request is rejected by server on status code 404.
     * @throws ResourceModifiedException thrown if the request is rejected by server on status code 409.
     * @return the {@link SyncPoller} for polling of long-running operation.
     */
    @ServiceMethod(returns = ReturnType.LONG_RUNNING_OPERATION)
    public SyncPoller<BinaryData, BinaryData> beginExport(
            String name, String projectFileVersion, RequestOptions requestOptions) {
        return this.beginExportAsync(name, projectFileVersion, requestOptions).getSyncPoller();
    }
}
