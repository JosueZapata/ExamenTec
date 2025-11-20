import { StoreResponseDtoPagedResult } from './storeResponseDtoPagedResult';
import { HttpStatusCode } from './httpStatusCode';


export interface StoreResponseDtoPagedResultHttpResponse { 
    result?: StoreResponseDtoPagedResult;
    readonly message?: string | null;
    statusCode?: HttpStatusCode;
    readonly isError?: boolean;
}
export namespace StoreResponseDtoPagedResultHttpResponse {
}



