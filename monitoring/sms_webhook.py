# sms_webhook.py - خدمة وسيطة لإرسال SMS عبر SMS Misr
from flask import Flask, request, jsonify
import requests
import os
import logging

app = Flask(__name__)
logging.basicConfig(level=logging.INFO)

# إعدادات SMS Misr
SMS_MISR_USERNAME = os.environ.get('SMS_MISR_USERNAME', 'your_username')
SMS_MISR_PASSWORD = os.environ.get('SMS_MISR_PASSWORD', 'your_password')
SMS_MISR_SENDER = os.environ.get('SMS_MISR_SENDER', 'YourApp')
TARGET_PHONE = os.environ.get('TARGET_PHONE', '01234567890')  # رقمك

@app.route('/webhook', methods=['POST'])
def send_sms():
    try:
        # استلام البيانات من Alertmanager
        data = request.json
        logging.info(f"Received alert: {data}")
        
        # استخراج معلومات التنبيه
        alerts = data.get('alerts', [])
        if not alerts:
            return jsonify({"error": "No alerts found"}), 400
        
        # تحضير رسالة SMS
        sms_messages = []
        for alert in alerts:
            alert_name = alert.get('labels', {}).get('alertname', 'Unknown')
            severity = alert.get('labels', {}).get('severity', 'unknown')
            summary = alert.get('annotations', {}).get('summary', 'No summary')
            instance = alert.get('labels', {}).get('instance', 'unknown')
            
            message = f"🚨 Alert: {alert_name}\nSeverity: {severity}\nInstance: {instance}\n{summary}"
            sms_messages.append(message)
        
        # دمج الرسائل (SMS Misr يدعم 160 حرف)
        combined_message = "\n---\n".join(sms_messages)
        if len(combined_message) > 160:
            combined_message = combined_message[:157] + "..."
        
        # إرسال SMS عبر SMS Misr API
        sms_payload = {
            'username': SMS_MISR_USERNAME,
            'password': SMS_MISR_PASSWORD,
            'language': '2',  # عربي + إنجليزي
            'sender': SMS_MISR_SENDER,
            'mobile': TARGET_PHONE,
            'message': combined_message
        }
        
        response = requests.post(
            'https://smsmisr.com/api/SMS/',
            data=sms_payload,
            headers={'Content-Type': 'application/x-www-form-urlencoded'},
            timeout=10
        )
        
        logging.info(f"SMS Misr response: {response.status_code} - {response.text}")
        
        if response.status_code == 200:
            return jsonify({
                "status": "success",
                "message": "SMS sent successfully",
                "sms_response": response.text
            })
        else:
            return jsonify({
                "status": "error",
                "message": f"SMS failed with status {response.status_code}",
                "sms_response": response.text
            }), 500
            
    except Exception as e:
        logging.error(f"Error sending SMS: {str(e)}")
        return jsonify({"error": str(e)}), 500

@app.route('/health', methods=['GET'])
def health():
    return jsonify({"status": "healthy"})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001, debug=True)