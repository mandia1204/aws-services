const axios = require('axios').default;
const jwt_decode = require('jwt-decode');
const jwkToPem = require('jwk-to-pem'),
    jwt = require('jsonwebtoken');

const publicKeysUrl = 'https://cognito-idp.us-east-2.amazonaws.com/us-east-xxxxx/.well-known/jwks.json';
let response;

/**
 *
 * Event doc: https://docs.aws.amazon.com/apigateway/latest/developerguide/set-up-lambda-proxy-integrations.html#api-gateway-simple-proxy-for-lambda-input-format
 * @param {Object} event - API Gateway Lambda Proxy Input Format
 *
 * Context doc: https://docs.aws.amazon.com/lambda/latest/dg/nodejs-prog-model-context.html 
 * @param {Object} context
 *
 * Return doc: https://docs.aws.amazon.com/apigateway/latest/developerguide/set-up-lambda-proxy-integrations.html
 * @returns {Object} object - API Gateway Lambda Proxy Output Format
 * 
 */
exports.lambdaHandler = async (event, context) => {
    try {
        const { kid } = jwt_decode(event.authorizationToken, { header: true});
        const { data } = await axios(publicKeysUrl);
        const jwk = data.keys.find(k => k.kid === kid);

        const pem = jwkToPem(jwk);
        const options = {
            audience: '',
            issuer: 'https://cognito-idp.us-east-2.amazonaws.com/xxxx',
            ignoreExpiration: false
        };
        const decodedToken = jwt.verify(event.authorizationToken, pem, options);

        response = {
            'statusCode': 200,
            'body': JSON.stringify({
                message: 'hello world',
                decodedToken
                // location: ret.data.trim()
            })
        }
    } catch (err) {
        console.log(err);
        return err;
    }

    return response
};
