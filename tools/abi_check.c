#include <stdio.h>
#include <stddef.h>
#include "webp/encode.h"
#include "webp/decode.h"
#include "webp/mux.h"
#include "webp/demux.h"

int main(void) {
    printf("{\n");
    printf("  \"encoder_abi\": \"0x%04x\",\n", WEBP_ENCODER_ABI_VERSION);
    printf("  \"decoder_abi\": \"0x%04x\",\n", WEBP_DECODER_ABI_VERSION);
    printf("  \"mux_abi\": \"0x%04x\",\n", WEBP_MUX_ABI_VERSION);
    printf("  \"demux_abi\": \"0x%04x\",\n", WEBP_DEMUX_ABI_VERSION);
    printf("  \"sizeof_WebPConfig\": %zu,\n", sizeof(WebPConfig));
    printf("  \"sizeof_WebPPicture\": %zu,\n", sizeof(WebPPicture));
    printf("  \"sizeof_WebPAuxStats\": %zu,\n", sizeof(WebPAuxStats));
    printf("  \"sizeof_WebPMemoryWriter\": %zu,\n", sizeof(WebPMemoryWriter));
    printf("  \"sizeof_WebPBitstreamFeatures\": %zu,\n", sizeof(WebPBitstreamFeatures));
    printf("  \"sizeof_WebPDecoderOptions\": %zu,\n", sizeof(WebPDecoderOptions));
    printf("  \"sizeof_WebPDecBuffer\": %zu,\n", sizeof(WebPDecBuffer));
    printf("  \"sizeof_WebPDecoderConfig\": %zu,\n", sizeof(WebPDecoderConfig));
    printf("  \"sizeof_WebPAnimEncoderOptions\": %zu,\n", sizeof(WebPAnimEncoderOptions));
    printf("  \"sizeof_WebPAnimDecoderOptions\": %zu,\n", sizeof(WebPAnimDecoderOptions));
    printf("  \"sizeof_WebPAnimInfo\": %zu,\n", sizeof(WebPAnimInfo));
    printf("  \"sizeof_WebPData\": %zu,\n", sizeof(WebPData));
    printf("  \"sizeof_WebPRGBABuffer\": %zu,\n", sizeof(WebPRGBABuffer));
    printf("  \"sizeof_WebPYUVABuffer\": %zu,\n", sizeof(WebPYUVABuffer));
    printf("  \"sizeof_WebPMuxAnimParams\": %zu,\n", sizeof(WebPMuxAnimParams));
    printf("  \"offsetof_WebPConfig_quality\": %zu,\n", offsetof(WebPConfig, quality));
    printf("  \"offsetof_WebPConfig_method\": %zu,\n", offsetof(WebPConfig, method));
    printf("  \"offsetof_WebPConfig_qmin\": %zu,\n", offsetof(WebPConfig, qmin));
    printf("  \"offsetof_WebPConfig_qmax\": %zu,\n", offsetof(WebPConfig, qmax));
    printf("  \"offsetof_WebPPicture_width\": %zu,\n", offsetof(WebPPicture, width));
    printf("  \"offsetof_WebPPicture_height\": %zu,\n", offsetof(WebPPicture, height));
    printf("  \"offsetof_WebPPicture_writer\": %zu,\n", offsetof(WebPPicture, writer));
    printf("  \"offsetof_WebPPicture_error_code\": %zu,\n", offsetof(WebPPicture, error_code));
    printf("  \"offsetof_WebPPicture_memory_\": %zu,\n", offsetof(WebPPicture, memory_));
    printf("  \"offsetof_WebPDecBuffer_u\": %zu,\n", offsetof(WebPDecBuffer, u));
    printf("  \"offsetof_WebPDecBuffer_private_memory\": %zu,\n", offsetof(WebPDecBuffer, private_memory));
    printf("  \"offsetof_WebPAuxStats_cross_color_transform_bits\": %zu,\n", offsetof(WebPAuxStats, cross_color_transform_bits));
    printf("  \"encoder_version\": \"0x%06x\",\n", WebPGetEncoderVersion());
    printf("  \"decoder_version\": \"0x%06x\",\n", WebPGetDecoderVersion());
    printf("  \"pointer_size\": %zu\n", sizeof(void*));
    printf("}\n");
    return 0;
}
