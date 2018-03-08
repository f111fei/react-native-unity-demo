import * as React from 'react';
import { View, Text, TouchableWithoutFeedback, StyleSheet, ViewStyle, TextStyle } from 'react-native';

export interface ButtonProps {
    style?: ViewStyle;
    labelStyle?: TextStyle;
    enabled?: boolean;
    label?: string;
    onPress?: () => void;
}

export default class Button extends React.Component<ButtonProps, any> {
    public static defaultProps: ButtonProps = {
        enabled: true
    };

    public render() {
        return (
            <TouchableWithoutFeedback onPress={() => this.props.onPress && this.props.onPress()}>
                <View style={[styles.button, this.props.style, { opacity: this.props.enabled ? 1 : 0.5 }]}>
                    <Text style={[styles.label, this.props.labelStyle]}>{this.props.label}</Text>
                </View>
            </TouchableWithoutFeedback>
        );
    }
}

const styles = StyleSheet.create({
    button: {
        width: 120,
        minWidth: 100,
        height: 30,
        borderRadius: 4,
        backgroundColor: '#448cfe',
        justifyContent: 'center',
        alignItems: 'center'
    } as ViewStyle,
    label: {
        color: '#fff',
        fontSize: 14
    } as TextStyle
});