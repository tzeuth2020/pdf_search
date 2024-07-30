// middleware.ts
import { NextResponse, NextRequest } from 'next/server';

const basicAuth = (req: NextRequest): { user: string; pass: string } | null => {
  const auth = req.headers.get('authorization');
  if (!auth) {
    console.log('No authorization header');
    return null;
  }

  const [scheme, encoded] = auth.split(' ');
  if (scheme !== 'Basic') {
    console.log('Authorization scheme is not Basic');
    return null;
  }

  const buffer = Buffer.from(encoded, 'base64');
  const [user, pass] = buffer.toString('utf-8').split(':');

  return { user, pass };
};

export function middleware(req: NextRequest) {
  const credentials = basicAuth(req);

  if (!credentials) {
    return new NextResponse('Unauthorized', {
      status: 401,
      headers: {
        'WWW-Authenticate': 'Basic realm="Secure Area"',
      },
    });
  }

  const { user, pass } = credentials;


  const validUser = process.env.BASIC_AUTH_USER;
  const validPass = process.env.BASIC_AUTH_PASS;

  if (user === validUser && pass === validPass) {
    return NextResponse.next();
  }

  console.log('Authentication failed');
  return new NextResponse('Unauthorized', {
    status: 401,
    headers: {
      'WWW-Authenticate': 'Basic realm="Secure Area"',
    },
  });
}

export const config = {
  matcher: ['/', '/index']
};
