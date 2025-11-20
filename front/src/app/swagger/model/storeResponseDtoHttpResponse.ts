import { StoreResponseDto } from './storeResponseDto';
import { HttpStatusCode } from './httpStatusCode';


export interface StoreResponseDtoHttpResponse { 
    result?: StoreResponseDto;
    readonly message?: string | null;
    statusCode?: HttpStatusCode;
    readonly isError?: boolean;
}
export namespace StoreResponseDtoHttpResponse {
}



