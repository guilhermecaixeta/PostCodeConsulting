﻿using Craftable.Core.interfaces.CQRS;
using Craftable.Core.interfaces.services;
using Craftable.SharedKernel.DTO;
using Craftable.SharedKernel.exceptions;
using Craftable.Web.DTO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Craftable.Web.services
{
    public class AddressMapperService : IAddressMapperService
    {

        public ResponseDTO<PostcodeDistanceDTO> MappingPostcode(ResultDTO<PostcodeRangedDTO> resultDTO)
        {
            var result = resultDTO.Data;
            var data = new PostcodeDistanceDTO
            {
                PostalCode = result.Code,
                DistanceInKilometer = result.DistanceFromHeathrowAirportInKilometer,
                DistanceInMiles = result.DistanceFromHeathrowAirportInMiles
            };

            var ResponseDTO = GetSuccessResponse(data, HttpStatusCode.Created);

            return ResponseDTO;
        }

        public ResponseDTO<IReadOnlyList<PostalcodeDTO>> MappingPostcodeList(IQueryResult<IReadOnlyList<PostcodeDTO>> queryResult)
        {

            if (!queryResult.Success)
            {
                return GetErrorResponse<IReadOnlyList<PostalcodeDTO>>(queryResult.Errors);
            }

            var addresses = queryResult.Data.Select(address => new PostalcodeDTO
            {
                Code = address.Code,
                Date = address.Date.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            });

            if (!addresses.Any())
            {
                return GetSuccessResponse<IReadOnlyList<PostalcodeDTO>>(default, HttpStatusCode.NoContent);
            }

            return GetSuccessResponse<IReadOnlyList<PostalcodeDTO>>(addresses.ToList(), HttpStatusCode.OK);
        }

        private static ResponseDTO<T> GetErrorResponse<T>(IReadOnlyList<string> errors) where T : class =>
            new ResponseDTO<T>
            {
                Data = default,
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Errors = errors
            };

        private static ResponseDTO<T> GetSuccessResponse<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK) where T : class =>
            new ResponseDTO<T>
            {
                Data = data,
                Success = false,
                StatusCode = statusCode,
                Errors = default
            };
    }
}