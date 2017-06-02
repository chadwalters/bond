// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include <memory>
#include <grpc++/impl/codegen/status.h>
#include <grpc++/client_context.h>

namespace bond { namespace ext { namespace gRPC {

    template <typename TResponse>
    struct client_callback_args
    {
        client_callback_args(
            const bond::bonded<TResponse>& response,
            const grpc::Status& status,
            std::shared_ptr<grpc::ClientContext> context)
            : _response(response), _status(status), _context(context)
        {}

        bond::bonded<TResponse> _response;
        const grpc::Status& _status;
        std::shared_ptr<grpc::ClientContext> _context;
    };

} } } // namespace bond::ext::gRPC
