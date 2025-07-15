import logger from "~/services/logger/logger.server";
import { v4 as uuidv4 } from 'uuid';
import { Auth } from "~/constants";

interface IResponse {
  requestId: string | null,
  userId: string | null,
  route: string
  data?: any
  error?: any
  message?: string | null
  status?: number
}

/**
 * Response Handler for API Response
 * @param IResponse object 
 * @returns Response
 */
export const responseHandler = ({
  requestId,
  userId,
  route,
  data = null,
  error = null,
  message = null,
  status = 200,
}: IResponse) => {

  if(!requestId)
    requestId = uuidv4();

  if(!userId)
    userId = Auth.EMPTY_USER_UUID;
  
  if (error)
    logger.error(JSON.stringify(error), { requestId, userId, route, data, message, status });

  // if (error instanceof z.ZodError) {
  //   return responseHandler({error: zodErrorMessage(error), status: 400})
  // }

  // if (
  //   error === LOGOUT_ERROR_MESSAGE ||
  //   error?.message === LOGOUT_ERROR_MESSAGE
  // ) {
  //   return json(
  //     {error: error?.message ?? error, status: 403},
  //     {
  //       status: 403,
  //       headers: {
  //         'Clear-Site-Data': 'cookies',
  //       },
  //     },
  //   )
  // }

  // if (
  //   error === ACCESS_ERROR_MESSAGE ||
  //   error?.message === ACCESS_ERROR_MESSAGE
  // ) {
  //   return json(
  //     {error: error?.message ?? error, status: 401},
  //     {
  //       status: 401,
  //       headers: {
  //         'Clear-Site-Data': 'cookies',
  //       },
  //     },
  //   )
  // }
  // if (error === 'Unexpected end of JSON input') {
  //   return json({error: 'Request body is missing', status: 400}, {status: 400})
  // }

  return new Response(
    JSON.stringify({ data, error, status }),
    {
      status,
      headers: { "Content-Type": "application/json" }
    }
  )
}

