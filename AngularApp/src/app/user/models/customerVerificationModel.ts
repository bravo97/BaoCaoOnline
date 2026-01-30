// Response model for customer key verification
// API trả về { "id": "guid" } nếu tồn tại
// Hoặc error 404/400 nếu không tồn tại
export interface CustomerVerificationResponse {
    id: string;
}
