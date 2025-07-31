from flask import Flask, request, jsonify
from twilio.rest import Client
import os

app = Flask(__name__)

# إعداد بيانات Twilio من environment variables
ACCOUNT_SID = os.environ.get('TWILIO_ACCOUNT_SID')
AUTH_TOKEN = os.environ.get('TWILIO_AUTH_TOKEN')
FROM_WHATSAPP = 'whatsapp:+14155238886'
TO_WHATSAPP = os.environ.get('TARGET_WHATSAPP', 'whatsapp:+201012345504')  # رقمك

client = Client(ACCOUNT_SID, AUTH_TOKEN)

@app.route('/webhook', methods=['POST'])
def receive_alert():
    try:
        data = request.json
        alerts = data.get("alerts", [])
        if not alerts:
            return jsonify({"error": "No alerts found"}), 400
        
        for alert in alerts:
            alertname = alert.get("labels", {}).get("alertname", "Unnamed Alert")
            severity = alert.get("labels", {}).get("severity", "unknown")
            summary = alert.get("annotations", {}).get("summary", "No summary")
            instance = alert.get("labels", {}).get("instance", "unknown")

            body = f"""
*🚨 Alert:* {alertname}
*Severity:* {severity}
*Instance:* {instance}
{summary}
"""

            message = client.messages.create(
                body=body.strip(),
                from_=FROM_WHATSAPP,
                to=TO_WHATSAPP
            )
        
        return jsonify({"status": "success", "message": "Alert sent via WhatsApp"}), 200
    
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

@app.route('/test', methods=['GET'])
def test_send():
    try:
        message = client.messages.create(
            body="🌐 Test WhatsApp message from webhook Flask app",
            from_=FROM_WHATSAPP,
            to=TO_WHATSAPP
        )
        return jsonify({"status": "success", "sid": message.sid})
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001)
