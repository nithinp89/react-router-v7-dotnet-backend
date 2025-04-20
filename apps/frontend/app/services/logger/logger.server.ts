import winston from 'winston';
import 'winston-daily-rotate-file';
import os from 'os';

// Determine if we're in production mode
const isProduction = process.env.NODE_ENV === 'production';

// Get machine ID (hostname)
const machineId = os.hostname();

// Configure the daily rotate transport for file logging
// Use type assertion to bypass TypeScript error
const fileRotateTransport = new (winston.transports as any).DailyRotateFile({
  filename: 'logs/frontend-app-%DATE%.log',
  datePattern: 'YYYY-MM-DD',
  maxFiles: '60d', // Keep logs for 60 days
  maxSize: '5m', // Maximum size of each log file
  format: winston.format.combine(
    winston.format.timestamp({
      format: 'YYYY-MM-DD HH:mm:ss'
    }),
    winston.format.errors({ stack: true }),
    winston.format.splat(),
    winston.format.json()
  )
});

// Create the logger instance
const logger = winston.createLogger({
  level: isProduction ? 'info' : 'debug',
  format: winston.format.combine(
    winston.format.timestamp({
      format: 'YYYY-MM-DD HH:mm:ss'
    }),
    winston.format.errors({ stack: true }),
    winston.format.splat(),
    winston.format.json()
  ),
  defaultMeta: { 
    service: 'frontend-app',
    machineId,
  },
  transports: [
    fileRotateTransport
  ]
});

// Add console transport in non-production environments
if (!isProduction) {
  logger.add(new (winston.transports as any).Console({
    format: winston.format.combine(
      winston.format.colorize(),
      winston.format.printf(({ timestamp, level, message, ...meta }) => {
        return `${timestamp} ${level}: ${message} ${Object.keys(meta).length ? JSON.stringify(meta, null, 2) : ''}`;
      })
    )
  }));
}

// Export the logger instance
export default logger;
